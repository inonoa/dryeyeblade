using System;
using System.Linq;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

public class ScoreCounter : MonoBehaviour
{
    ReactiveProperty<int> _Score = new ReactiveProperty<int>(0);
    public IReadOnlyReactiveProperty<int> Score => _Score;
    
    Subject<ScoreInfo> _ScoreAdded = new Subject<ScoreInfo>();
    public IObservable<ScoreInfo> ScoreAdded => _ScoreAdded;

    void Start()
    {
        Hero.Current.Where(hero => hero != null).Subscribe(hero =>
        {
            hero.Attack.KilledEnemies.Subscribe(info =>
            {
                foreach (var enemy in info.enemies)
                {
                    _ScoreAdded.OnNext(new ScoreInfo(enemy, info.enemies.Count));
                }

                _Score.Value += info.enemies.Count * info.enemies.Sum(enemy => enemy.Score);
            });
        });
    }

    public void Reset() => _Score.Value = 0;
}

public class ScoreInfo
{
    public readonly IDamageable target;
    public readonly int rate;

    public ScoreInfo(IDamageable target, int rate)
    {
        (this.rate, this.target) = (rate, target);
    }
}
