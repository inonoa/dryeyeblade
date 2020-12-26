using System;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Serialization;

public class ZakoAttack : MonoBehaviour, IDoOnTimeStopped
{
    [FormerlySerializedAs("collider")] [SerializeField] Collider2D attackCollider;
    [SerializeField] Collider2D sensor;

    Hero target;
    
    public bool CanAttack => nearHero && !isAttacking;
    bool nearHero = false;
    bool isAttacking = false;

    void Start()
    {
        sensor.OnTriggerEnter2DAsObservable()
            .Where(other => other.CompareTag("HeroLife"))
            .Subscribe(other =>
            {
                nearHero = true;
                target = other.GetComponentInParent<Hero>();
            })
            .AddTo(this);
        sensor.OnTriggerExit2DAsObservable()
            .Where(other => other.CompareTag("HeroLife"))
            .Subscribe(other =>
            {
                nearHero = false;
                target = null;
            })
            .AddTo(this);
    }

    List<Tween> tweensInAttack = new List<Tween>();
    Subject<Unit> currentAttackEnded;
    public IObservable<Unit> Attack()
    {
        tweensInAttack = new List<Tween>();
        
        isAttacking = true;
        currentAttackEnded = new Subject<Unit>();

        tweensInAttack.Add(DOVirtual.DelayedCall(1.5f, () => attackCollider.enabled = true).ReactsToHeroEye());
        tweensInAttack.Add(DOVirtual.DelayedCall(2.3f, () => attackCollider.enabled = false).ReactsToHeroEye());
        tweensInAttack.Add(DOVirtual.DelayedCall(2.5f, () => currentAttackEnded.OnNext(Unit.Default)).ReactsToHeroEye());
        tweensInAttack.Add(DOVirtual.DelayedCall(3f,   () => isAttacking = false).ReactsToHeroEye());

        return currentAttackEnded;
    }

    public void ForceStopAttack()
    {
        tweensInAttack.ForEach(tw => tw.Kill());
        isAttacking = false;
        attackCollider.enabled = false;
        currentAttackEnded?.OnNext(Unit.Default);
    }

    bool colliderEnabledBeforeStop = false;
    public void OnTimeStopped()
    {
        colliderEnabledBeforeStop = attackCollider.enabled;
        attackCollider.enabled = false;
    }

    public void OnTimeRestarted()
    {
        attackCollider.enabled = colliderEnabledBeforeStop;
    }
}