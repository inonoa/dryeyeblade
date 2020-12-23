using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class HeroAttack : SerializedMonoBehaviour
{
    [SerializeField] Collider2D collier;
    
    //これViewっしょ…………
    [SerializeField] DamageEffect damageEffectPrefab;
    
    [SerializeField, ReadOnly] List<AttackHitInfo> hits = new List<AttackHitInfo>();

    HeroParams param;
    Hero hero;

    public void Init(Hero hero, HeroParams param)
    {
        this.param = param;
        this.hero = hero;
        hero.EyesAreOpen
            .Subscribe(open =>
            {
                if(open) OnEyesOpen();
                _CanAttack = !open;
            })
            .AddTo(this);
    }

    bool _CanAttack = false;
    bool CanAttack => _CanAttack;
    
    Subject<Unit> _OnAttack = new Subject<Unit>();
    public IObservable<Unit> OnAttack => _OnAttack;

    void Start()
    {
        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(param.AttackKey))
            .Where(_ => CanAttack)
            .ThrottleFirst(TimeSpan.FromSeconds(0.5f))
            .Subscribe(_ => Attack())
            .AddTo(this);
    }

    void Attack()
    {
        collier.enabled = true;
        DOVirtual.DelayedCall(0.3f, FinishAttack);
        _OnAttack.OnNext(Unit.Default);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Damageable"))
        {
            hits.Add(new AttackHitInfo
            (
                other.ClosestPoint(hero.transform.position),
                hero.KeyDirection, 
                other.GetComponentInParent<IDamageable>()
            ));
        }
    }

    Subject<Unit> _OnAttackFinished = new Subject<Unit>();
    public Subject<Unit> OnAttackFinished => _OnAttackFinished;
    void FinishAttack()
    {
        collier.enabled = false;
        _OnAttackFinished.OnNext(Unit.Default);
    }

    void OnEyesOpen()
    {
        hits.ForEach(hit =>
        {
            hit.Target.Damage(param.NormalDamage);
            Instantiate(damageEffectPrefab, hit.HitPos, Quaternion.identity)
                .Play(hit.AttackDir);
        });
        hits = new List<AttackHitInfo>();
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

public interface IDamageable
{
    void Damage(float damage);
}
