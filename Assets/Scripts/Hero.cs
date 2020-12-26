using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

public class Hero : MonoBehaviour
{
    [SerializeField] HeroParams param;
    [SerializeField] HeroAttack attack;
    [SerializeField] HeroLife life;
    [SerializeField] Rigidbody2D rigidBody;
    [SerializeField] HeroEye eye;

    public HeroParams Param => param;
    
    public enum EState{ Normal, Attacking, Damaged, Dead }

    ReactiveProperty<EState> _State = new ReactiveProperty<EState>(EState.Normal);
    public IObservable<Unit> OnDamaged 
        => _State
            .Where(state => state == EState.Damaged || state == EState.Dead)
            .Select(_ => Unit.Default);
    public IObservable<Unit> OnDeath
        => _State
            .Where(state => state == EState.Dead)
            .Select(_ => Unit.Default);

    public IReadOnlyReactiveProperty<EState> State => _State;

    public HeroEye Eye => eye;
    public HeroLife Life => life;

    ReactiveProperty<Dir8> _KeyDirection = new ReactiveProperty<Dir8>(Dir8.None);
    public IReadOnlyReactiveProperty<Dir8> KeyDirection => _KeyDirection;
    public IReadOnlyReactiveProperty<Dir8> EyeDirection { get; private set; }


    [SerializeField] Vector2 speed = Vector2.zero;

    [SerializeField, ReadOnly] bool canMove = true;

    public IReadOnlyReactiveProperty<bool> EyesAreOpen => eye.IsOpen;

    void Awake()
    {
        attack.Init(this, param);

        ReactiveProperty<Dir8> eyeDir = new ReactiveProperty<Dir8>(Dir8.D);
        KeyDirection
            .Where(key => key != Dir8.None)
            .Subscribe(key => eyeDir.Value = key);
        EyeDirection = eyeDir;

        attack.OnAttack.Subscribe(_ => _State.Value = EState.Attacking);
        attack.OnAttackFinished.Subscribe(_ => _State.Value = EState.Normal);
    }

    void Start()
    {
        life.Life.Subscribe(lf =>
        {
            if (lf == life.LifeMax) return;
            if (lf == 0)
            {
                canMove = false;
                speed = Vector2.zero;
                rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
                _State.Value = EState.Dead;
                return;
            }

            rigidBody
                .DOMove(- EyeDirection.Value.ToVec2() * param.KnockBackDistance, param.StunTime)
                .SetRelative()
                .SetEase(Ease.OutQuad)
                .OnComplete(() => _State.Value = EState.Normal);
            _State.Value = EState.Damaged;
        });
        
        _Current.Value = this;
    }

    void Update()
    {
        UpdateMove();
    }

    void UpdateMove()
    {
        if(!canMove) return;
        
        float horInput  = Input.GetAxisRaw("Horizontal");
        float vertInput = Input.GetAxisRaw("Vertical");
        
        _KeyDirection.Value = new Vector2(horInput, vertInput).ToDir8();
        
        if(_State.Value != EState.Normal) return;
        
        if (horInput == 0 && vertInput == 0)
        {
            float magResistance = param.Resistance * Time.deltaTime;
            
            if (magResistance > speed.magnitude) speed = Vector2.zero;
            else speed -= speed.normalized * magResistance;
        }
        else
        {
            speed += new Vector2(horInput, vertInput)
                .normalized * (param.RunForce * Time.deltaTime);
        }
        
        if (speed.sqrMagnitude > param.SpeedMax * param.SpeedMax)
        {
            speed = speed.normalized * param.SpeedMax;
        }
    }

    void FixedUpdate()
    {
        if(!canMove) return;
        if(attack.IsAttacking) return;
        if(_State.Value == EState.Damaged) return;

        rigidBody.MovePosition(transform.position + speed.Vec3() * Time.fixedDeltaTime);
    }

    public bool CanBeDamaged() => ! (_State.Value == EState.Damaged || _State.Value == EState.Dead);
    
    static ReactiveProperty<Hero> _Current = new ReactiveProperty<Hero>();
    public static IObservable<Hero> CurrentSet => _Current;
}
