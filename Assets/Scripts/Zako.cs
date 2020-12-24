using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

public class Zako : MonoBehaviour, IDamageable, IDoOnTimeStopped
{
    public enum EState{ Wandering, ChaseHero, Attack, Dead }
    [SerializeField, ReadOnly] EState state = EState.Wandering;
    public EState State => state;
    
    ReactiveProperty<Dir8> _WanderDir = new ReactiveProperty<Dir8>();
    public Dir8 WanderDir => _WanderDir.Value;
    public IObservable<Dir8> WanderDirSet => _WanderDir;

    [SerializeField] Rigidbody2D rigidBody;
    [SerializeField] ZakoAttack attack;
    [SerializeField] float chaseSpeed = 3;
    [SerializeField] float wanderSpeed = 2;
    [SerializeField] int _Score = 100;
    public int Score => _Score;

    Hero targetHero;

    void Start()
    {
        ChangeDir();
        
        DOVirtual.DelayedCall
        (
            Random.Range(0, 2f),
            () => Observable
                  .Interval(TimeSpan.FromSeconds(2))
                  .Where(_ => state == EState.Wandering)
                  .Subscribe(_ => ChangeDir())
                  .AddTo(this)
        );
    }

    void ChangeDir()
    {
        _WanderDir.Value = Dir8Extension.Random();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(! other.CompareTag("Hero")) return;

        targetHero = other.GetComponentInParent<Hero>();
        
        if (state == EState.Wandering)
        {
            state = EState.ChaseHero;
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if(! other.CompareTag("Hero")) return;
        
        if (state == EState.ChaseHero)
        {
            state = EState.Wandering;
        }
    }
    

    public Vector2 RunDir { get; private set; }
    void FixedUpdate()
    {
        switch (state)
        {
        case EState.Wandering:
        {
            rigidBody.MovePosition(transform.position + WanderDir.ToVec2().Vec3() * (wanderSpeed * Time.deltaTime));
        }
        break;
        case EState.ChaseHero:
        {
            Vector2 thisToHero = targetHero.transform.position - this.transform.position;
            RunDir = thisToHero.normalized;
            rigidBody.MovePosition(transform.position + RunDir.Vec3() * (chaseSpeed * Time.deltaTime));

            if (attack.CanAttack)
            {
                attack.Attack()
                    .Delay(TimeSpan.FromSeconds(1))
                    .Subscribe(_ => state = EState.ChaseHero);
                state = EState.Attack;
            }
        }
        break;
        }
    }
    

    Subject<Unit> _OnDamaged = new Subject<Unit>();
    public IObservable<Unit> OnDamaged => _OnDamaged;
    
    Subject<Unit> _OnDeath = new Subject<Unit>();
    public IObservable<Unit> OnDeath => _OnDeath;
    public void Damage(float damage)
    {
        state = EState.Dead;
        _OnDamaged.OnNext(Unit.Default);
        _OnDeath.OnNext(Unit.Default);
    }

    /// <summary>
    /// だいぶん気持ち悪い(呼び損ねる)
    /// </summary>
    public void OnDamageDisplayEnded()
    {
        Destroy(gameObject);
    }

    public void OnTimeStopped()
    {
        this.enabled = false;
        rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    public void OnTimeRestarted()
    {
        this.enabled = true;
        rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}
