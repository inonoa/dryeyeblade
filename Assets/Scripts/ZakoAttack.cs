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

    Hero target;
    public bool CanAttack { get; private set; } = false;

    void Start()
    {
        sensor.OnTriggerEnter2DAsObservable()
            .Where(other => other.CompareTag("Hero"))
            .Subscribe(other =>
            {
                CanAttack = true;
                target = other.GetComponentInParent<Hero>();
            })
            .AddTo(this);
        sensor.OnTriggerExit2DAsObservable()
            .Where(other => other.CompareTag("Hero"))
            .Subscribe(other =>
            {
                CanAttack = false;
                target = null;
            })
            .AddTo(this);
    }

    public IObservable<Unit> Attack()
    {
        attackCollider.enabled = true;
        
        Subject<Unit> onFinished = new Subject<Unit>();
        DOVirtual.DelayedCall(0.3f, () =>
        {
            attackCollider.enabled = false;
            onFinished.OnNext(Unit.Default);
        });

        return onFinished;
    }
}