using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class Boss : MonoBehaviour, IDamageable, IDoOnTimeStopped
{
    [SerializeField] BoxCollider2D mainCollider;
    [SerializeField] float mainColliderOffsetInAttack = -1;
    [SerializeField] Vector2 mainColliderSizeInAttack;
    [SerializeField] Collider2D attackCollider;
    [SerializeField] Collider2D heroSensor;
    [SerializeField] ContactFilter2D heroFilter;
    
    public enum EState
    {
        Normal, Attacking, Damaged, Dead
    }
    
    [SerializeField, ReadOnly] EStateReactiveProperty _State = new EStateReactiveProperty(EState.Normal);
    public IReadOnlyReactiveProperty<EState> State => _State;

    [SerializeField, ReadOnly] int _HP = 10;
    public bool Damage(float damage)
    {
        if (_State.Value == EState.Dead) return false;
        
        _HP--;
        if (_HP == 0)
        {
            _State.Value = EState.Dead;
            return true;
        }
        else
        {
            _State.Value = EState.Damaged;
            return false;
        }
    }

    [SerializeField] int _Score = 5000;
    public int Score => _Score;


    public IObservable<Unit> OnDeath => State.Where(state => state == EState.Dead).Select(_ => Unit.Default);

    void Awake()
    {
        State.Subscribe(state =>
        {
            switch (state)
            {
            case EState.Damaged:
                DOVirtual.DelayedCall(0.5f, () =>
                {
                    if(State.Value != EState.Damaged) return;
                    
                    _State.Value = EState.Normal;
                    if (heroSensor.IsTouching(heroFilter))
                    {
                        DOVirtual.DelayedCall(1f, () => _State.Value = EState.Attacking);
                    }
                });
                break;

            case EState.Dead:
                DOVirtual.DelayedCall(1f, () => Destroy(gameObject));
                break;
            
            case EState.Attacking:
                
                Vector2 normalSize = mainCollider.size;
                float normalOffset = mainCollider.offset.y;
                
                Tween attackTween = DOTween.Sequence()
                    .AppendInterval(1.05f)
                    .AppendCallback(() =>
                    {
                        attackCollider.enabled = true;
                        mainCollider.size = mainColliderSizeInAttack;
                        mainCollider.offset = new Vector2(0, mainColliderOffsetInAttack);
                    })
                    .AppendInterval(0.3f)
                    .AppendCallback(() =>
                    {
                        attackCollider.enabled = false;
                    })
                    .AppendInterval(1.5f)
                    .AppendCallback(() =>
                    {
                        mainCollider.size = normalSize;
                        mainCollider.offset = new Vector2(0, normalOffset);
                    })
                    .AppendInterval(1.1f)
                    .AppendCallback(() =>
                    {
                        _State.Value = EState.Normal;
                        if (heroSensor.IsTouching(heroFilter))
                        {
                            DOVirtual.DelayedCall(1f, () => _State.Value = EState.Attacking);
                        }
                    });
                tweens.Add(attackTween);
                State.SkipLatestValueOnSubscribe()
                    .Take(2)
                    .Where(st => st != EState.Normal && st != EState.Attacking)
                    .Subscribe(_ =>
                    {
                        attackTween?.Kill();
                        attackCollider.enabled = false;
                        mainCollider.size = normalSize;
                        mainCollider.offset = new Vector2(0, normalOffset);
                    })
                    .AddTo(this);
                break;
            }
        })
        .AddTo(this);

        heroSensor.OnTriggerEnter2DAsObservable()
            .Where(col => col.CompareTag("HeroLife"))
            .Where(_ => State.Value == EState.Normal)
            .Subscribe(_ => _State.Value = EState.Attacking);

        Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(_ =>
        {
            tweens = tweens.Where(tw => tw.IsActive()).ToList();
        })
        .AddTo(this);
    }
    
    List<Tween> tweens = new List<Tween>();

    public void OnTimeStopped()
    {
        tweens.ForEach(tw => tw?.Pause());
    }

    public void OnTimeRestarted()
    {
        tweens.ForEach(tw => tw?.TogglePause());
    }
}

[Serializable]
public class EStateReactiveProperty : ReactiveProperty<Boss.EState>
{
    public EStateReactiveProperty()
    {
        //
    }

    public EStateReactiveProperty(Boss.EState init) : base(init)
    {
        //
    }
}
