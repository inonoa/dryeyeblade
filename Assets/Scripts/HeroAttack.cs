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
    
    [SerializeField, ReadOnly] List<IDamageable> hits = new List<IDamageable>();

    HeroParams param;
    Hero hero;

    public void Init(Hero hero, HeroParams param)
    {
        this.param = param;
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
            hits.Add(other.GetComponentInParent<IDamageable>());
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
        hits.ForEach(hit => hit.Damage(param.NormalDamage));
        hits = new List<IDamageable>();
    }
}

public interface IDamageable
{
    void Damage(float damage);
}
