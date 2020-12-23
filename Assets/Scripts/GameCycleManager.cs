﻿using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class GameCycleManager : MonoBehaviour
{
    [SerializeField] EnemyGroupsSpawner enemyGroupsSpawner;
    [SerializeField] Hero heroPrefab;
    [SerializeField] ResultScene resultScene;

    Hero lastHero;

    void Start()
    {
        StartGame();
        resultScene.Restart.Subscribe(_ => StartGame());
    }

    void StartGame()
    {
        enemyGroupsSpawner.StartSpawn();
        
        if(lastHero != null) Destroy(lastHero.gameObject);
        lastHero = Instantiate(heroPrefab);
        lastHero.OnDie.Subscribe(_ =>
            {
                enemyGroupsSpawner.StopSpawn();
                resultScene.Enter();
            })
            .AddTo(this);
    }
}