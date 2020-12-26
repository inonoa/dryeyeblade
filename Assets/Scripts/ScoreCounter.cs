using System;
using System.Linq;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

public class ScoreCounter : MonoBehaviour
{
    ReactiveProperty<int> _Score = new ReactiveProperty<int>(0);
    public IReadOnlyReactiveProperty<int> Score => _Score;

    void Start()
    {
        Hero.Current.Where(hero => hero != null).Subscribe(hero =>
        {
            hero.Attack.KilledEnemies.Subscribe(info =>
            {
                _Score.Value += CalcScore(info);
            });
        });
    }

    int CalcScore(KilledEnemiesInfo info)
    {
        return info.enemies.Count * info.enemies.Sum(enemy => enemy.Score);
    }

    public void Reset() => _Score.Value = 0;
}