using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

public class Zako : MonoBehaviour, IDamageable, IDoOnTimeStopped
{
    enum State{ Wandering, ChaseHero, Attack }
    [SerializeField] State state = State.Wandering;
    
    Dir8 wanderDir;

    [SerializeField] Rigidbody2D rigidBody;
    [SerializeField] ZakoAttack attack;
    [SerializeField] float chaseSpeed = 3;
    [SerializeField] float wanderSpeed = 2;

    Hero targetHero;

    void Start()
    {
        wanderDir = (Dir8) Random.Range(0, 8);
        
        DOVirtual.DelayedCall
        (
            Random.Range(0, 0.2f),
            () => Observable
                  .Interval(TimeSpan.FromSeconds(2))
                  .Where(_ => state == State.Wandering)
                  .Subscribe(_ => wanderDir = (Dir8) Random.Range(0, 8))
                  .AddTo(this)
        );
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(! other.CompareTag("Hero")) return;

        targetHero = other.GetComponentInParent<Hero>();
        
        if (state == State.Wandering)
        {
            state = State.ChaseHero;
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if(! other.CompareTag("Hero")) return;
        
        if (state == State.ChaseHero)
        {
            state = State.Wandering;
        }
    }
    

    void FixedUpdate()
    {
        switch (state)
        {
        case State.Wandering:
        {
            rigidBody.MovePosition(transform.position + wanderDir.ToVec2().Vec3() * (wanderSpeed * Time.deltaTime));
        }
        break;
        case State.ChaseHero:
        {
            Vector2 thisToHero = targetHero.transform.position - this.transform.position;
            rigidBody.MovePosition(transform.position + thisToHero.normalized.Vec3() * (chaseSpeed * Time.deltaTime));

            if (attack.CanAttack)
            {
                attack.Attack()
                    .Delay(TimeSpan.FromSeconds(1))
                    .Subscribe(_ => state = State.ChaseHero);
                state = State.Attack;
            }
        }
        break;
        case State.Attack:
        {
            //
        }
        break;
        }
    }
    

    public void Damage(float damage)
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
