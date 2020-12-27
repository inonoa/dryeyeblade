using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class GameCycleManager : MonoBehaviour
{
    [SerializeField] EnemyGroupsSpawner enemyGroupsSpawner;
    [SerializeField] ScoreCounter scoreCounter;
    [SerializeField] Hero heroPrefab;
    [SerializeField] Transform heroSpawnPosition;
    [SerializeField] TitleScene titleScene;
    [SerializeField] ResultScene resultScene;
    [SerializeField] new AudioSource audio;

    Hero lastHero;

    void Start()
    {
        titleScene.StartGame.Subscribe(_ => StartGame());
        resultScene.Restart.Subscribe(_ => StartGame());
    }

    void StartGame()
    {
        enemyGroupsSpawner.StartSpawn();
        scoreCounter.Reset();
        
        if(lastHero != null) Destroy(lastHero.gameObject);
        lastHero = Instantiate(heroPrefab, heroSpawnPosition.position, Quaternion.identity);
        lastHero.OnDeath.Subscribe(_ =>
            {
                enemyGroupsSpawner.StopSpawn();
                resultScene.Enter();
                audio.Stop();
            })
            .AddTo(this);

        audio.clip = SoundDatabase.Instance.bgmMain;
        audio.Play();
    }
}