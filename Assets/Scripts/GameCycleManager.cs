using System;
using DG.Tweening;
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
    [SerializeField] Animator blind;
    [SerializeField] GameObject IngameUIParent;
    [SerializeField] HeroLifeView heroLifeView;

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
            audio.Stop();
            blind.enabled = true;
            DOVirtual.DelayedCall(0.5f, () => blind.Play("eyeEffect_UI_blink", 0, 0));
            DOVirtual.DelayedCall(1f,   () => resultScene.Enter());
        })
        .AddTo(this);

        audio.clip = SoundDatabase.Instance.bgmMain;
        audio.Play();
        IngameUIParent.SetActive(true);
        heroLifeView.gameObject.SetActive(true);
    }
}