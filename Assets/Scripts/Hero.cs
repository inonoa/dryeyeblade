using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class Hero : MonoBehaviour
{
    [SerializeField] HeroParams param;
    [SerializeField] HeroAttack attack;
    [SerializeField] HeroLife life;
    [SerializeField] Rigidbody2D rigidBody;
    [SerializeField] HeroEye eye;

    public HeroEye Eye => eye;
    public HeroLife Life => life;

    ReactiveProperty<Dir8> _KeyDirection = new ReactiveProperty<Dir8>(Dir8.D);
    public IObservable<Dir8> KeyDirection => _KeyDirection;

    Vector2 speed = Vector2.zero;

    bool canMove = true;

    public IObservable<bool> EyesAreOpen => eye.IsOpenChanged;

    Subject<Unit> _OnDie = new Subject<Unit>();
    public IObservable<Unit> OnDie => _OnDie;

    void Awake()
    {
        attack.Init(this, param);
        _Current.Value = this;
    }

    void Start()
    {
        life.Life
            .Where(val => val == 0)
            .Subscribe(_ =>
            {
                canMove = false;
                speed = Vector2.zero;
                _OnDie.OnNext(Unit.Default);
            });
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
        rigidBody.MovePosition(transform.position + speed.Vec3() * Time.fixedDeltaTime);
    }
    
    static ReactiveProperty<Hero> _Current = new ReactiveProperty<Hero>();
    public static IObservable<Hero> CurrentSet => _Current;
    public static Hero Current => _Current.Value;
}
