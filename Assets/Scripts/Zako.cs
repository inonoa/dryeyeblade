using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

public class Zako : MonoBehaviour, IDamageable, IDoOnTimeStopped
{
    enum State{ Wandering, ChaseHero, Attack, Dead }
    [SerializeField] State state = State.Wandering;
    
    Dir8 wanderDir;

    [SerializeField] Rigidbody2D rigidBody;
    [SerializeField] ZakoAttack attack;
    [SerializeField] float chaseSpeed = 3;
    [SerializeField] float wanderSpeed = 2;
    [SerializeField] SpriteRenderer spriteRenderer;

    Hero targetHero;

    void Start()
    {
        wanderDir = Dir8Extension.Random();
        
        DOVirtual.DelayedCall
        (
            Random.Range(0, 2f),
            () => Observable
                  .Interval(TimeSpan.FromSeconds(2))
                  .Where(_ => state == State.Wandering)
                  .Subscribe(_ => wanderDir = Dir8Extension.Random())
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
        }
    }
    

    public void Damage(float damage)
    {
        state = State.Dead;
        StartCoroutine(Blink(() => Destroy(gameObject)));
    }

    IEnumerator Blink(Action onEnd)
    {
        foreach (var _ in Enumerable.Range(0, 3))
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(0.13f);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(0.13f);
        }
        onEnd.Invoke();
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
