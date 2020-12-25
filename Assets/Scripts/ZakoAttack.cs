using System;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Serialization;

public class ZakoAttack : MonoBehaviour
{
    [FormerlySerializedAs("collider")] [SerializeField] Collider2D attackCollider;
    [SerializeField] Collider2D sensor;
    [SerializeField] Animator effectPrefab;

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

    public IObservable<Unit> Attack()
    {
        isAttacking = true;
        Subject<Unit> onFinished = new Subject<Unit>();

        //blind対応
        DOVirtual.DelayedCall(1.5f, () => attackCollider.enabled = true);
        DOVirtual.DelayedCall(2.3f, () => attackCollider.enabled = false);
        DOVirtual.DelayedCall(2.5f, () => onFinished.OnNext(Unit.Default));
        DOVirtual.DelayedCall(3f,   () => isAttacking = false);

        return onFinished;
    }
}