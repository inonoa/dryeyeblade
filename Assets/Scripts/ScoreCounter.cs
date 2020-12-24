using System;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

public class ScoreCounter : MonoBehaviour
{
    [SerializeField] EnemyGroupsSpawner enemyGroupsSpawner;
    
    ReactiveProperty<int> _Score = new ReactiveProperty<int>(0);
    public IReadOnlyReactiveProperty<int> Score => _Score;

    void Start()
    {
        enemyGroupsSpawner.Spawned
            .SelectMany(group => group.Enemies)
            .Subscribe(enemy =>
            {
                enemy.OnDeath.Subscribe(_ => _Score.Value += enemy.Score).AddTo(this);
            })
            .AddTo(this);
    }

    public void Reset() => _Score.Value = 0;
}