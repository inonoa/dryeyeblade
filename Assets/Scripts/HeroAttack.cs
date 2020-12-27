using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class HeroAttack : SerializedMonoBehaviour
{
    [SerializeField] Collider2D collier;
    
    //これViewっしょ…………
    [SerializeField] DamageEffect damageEffectPrefab;
    
    ReactiveCollection<AttackHitInfo> _Hits = new ReactiveCollection<AttackHitInfo>();
    public IReadOnlyReactiveCollection<AttackHitInfo> Hits => _Hits;

    HeroParams param;
    Hero hero;

    public void Init(Hero hero, HeroParams param)
    {
        this.param = param;
        this.hero = hero;
    }

    public bool IsAttacking { get; private set; } = false;
    bool CanAttack => hero.Eye.FullyClosed && !IsAttacking;
    
    Subject<Hero> _OnAttack = new Subject<Hero>();
    public IObservable<Hero> OnAttack => _OnAttack;

    void Start()
    {
        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(param.AttackKey))
            .Where(_ => CanAttack)
            .ThrottleFirst(TimeSpan.FromSeconds(0.5f))
            .Subscribe(_ => Attack())
            .AddTo(this);

        collier.OnTriggerEnter2DAsObservable()
            .Where(other => other.CompareTag("Damageable"))
            .Subscribe(other =>
            {
                _Hits.Add(new AttackHitInfo
                (
                    other.ClosestPoint(hero.transform.position),
                    hero.EyeDirection.Value,
                    other.GetComponentInParent<IDamageable>()
                ));
            })
            .AddTo(this);

        EnemiesTimeChanger.Current.OnTimeRestarted.Subscribe(_ => ApplyAllHits());
    }

    void Attack()
    {
        IsAttacking = true;
        collier.enabled = true;
        collier.transform.rotation = Quaternion.Euler(0, 0, hero.EyeDirection.Value.ToAngleDeg());
        DOVirtual.DelayedCall(param.AttackSeconds, FinishAttack);
        _OnAttack.OnNext(hero);
    }

    Subject<Unit> _OnAttackFinished = new Subject<Unit>();
    public Subject<Unit> OnAttackFinished => _OnAttackFinished;
    void FinishAttack()
    {
        IsAttacking = false;
        collier.enabled = false;
        _OnAttackFinished.OnNext(Unit.Default);
    }

    Subject<KilledEnemiesInfo> _KilledEnemies = new Subject<KilledEnemiesInfo>();
    public IObservable<KilledEnemiesInfo> KilledEnemies => _KilledEnemies;
    void ApplyAllHits()
    {
        List<IDamageable> killed = new List<IDamageable>();
        _Hits.ForEach(hit =>
        {
            bool died = hit.Target.Damage(param.NormalDamage);
            if(died) killed.Add(hit.Target);
            Instantiate(damageEffectPrefab, hit.HitPos, Quaternion.identity)
                .Play(hit.AttackDir);
        });
        _KilledEnemies.OnNext(new KilledEnemiesInfo(killed));
        _Hits.Clear();
    }
}

[Serializable]
public class AttackHitInfo
{
    [SerializeField, ReadOnly] Vector2 hitPos;
    [SerializeField, ReadOnly] Dir8 attackDir;
    [SerializeField, ReadOnly] IDamageable target;

    public Vector2 HitPos => hitPos;
    public Dir8 AttackDir => attackDir;
    public IDamageable Target => target;

    public AttackHitInfo(Vector2 pos, Dir8 attackDir, IDamageable target)
    {
        this.hitPos = pos;
        this.attackDir = attackDir;
        this.target = target;
    }
}

public class KilledEnemiesInfo
{
    public readonly IReadOnlyList<IDamageable> enemies;
    public KilledEnemiesInfo(IReadOnlyList<IDamageable> enemies)
    {
        this.enemies = enemies;
    }
}

public interface IDamageable
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="damage"></param>
    /// <returns>死んだかどうか</returns>
    bool Damage(float damage);
    int Score { get; }
    IObservable<Unit> OnDeath { get; }
}
