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
    
    ReactiveProperty<EState> _State = new ReactiveProperty<EState>(EState.Wandering);
    public IReadOnlyReactiveProperty<EState> State => _State;

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
                  .Where(_ => State.Value == EState.Wandering)
                  .Subscribe(_ => ChangeDir())
                  .AddTo(this)
        );
    }

    void ChangeDir()
    {
        _WanderDir.Value = Dir8Extension.Random();
    }

    bool seeingHero = false;
    void OnTriggerEnter2D(Collider2D other)
    {
        if(! other.CompareTag("HeroLife")) return;

        seeingHero = true;
        targetHero = other.GetComponentInParent<Hero>();
        
        if (State.Value == EState.Wandering)
        {
            Vector2 thisToHero = targetHero.transform.position - this.transform.position;
            RunDir = thisToHero.normalized;
            
            _State.Value = EState.ChaseHero;
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if(! other.CompareTag("HeroLife")) return;

        seeingHero = false;
        if (State.Value == EState.ChaseHero)
        {
            _State.Value = EState.Wandering;
        }
    }
    

    public Vector2 RunDir { get; private set; }
    void FixedUpdate()
    {
        switch (State.Value )
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
                    .Subscribe(_ =>
                    {
                        if (seeingHero) _State.Value = EState.ChaseHero;
                        else            _State.Value = EState.Wandering;
                    });
                _State.Value = EState.Attack;
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
        if(State.Value == EState.Dead) return;
        attack.ForceStopAttack();
        _State.Value = EState.Dead;
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
