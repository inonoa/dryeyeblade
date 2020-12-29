using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class TitleScene : MonoBehaviour
{
    [SerializeField] Button startButton;
    [SerializeField] Button rankingButton;
    [SerializeField] GameObject notOnCanvas;
    [SerializeField] Animator blind;
    [SerializeField] float startDelay = 0.5f;
    [SerializeField] Hero heroInTitle;
    [SerializeField] RankingPopup ranking;

    public IObservable<Unit> StartGame { get; private set; }

    void Awake()
    {
        var startGame = new Subject<Unit>();
        
        startButton.OnClickAsObservable().Subscribe(_ =>
        {
            blind.enabled = true;
            blind.Play("eyeEffect_UI_blink", 0, 0);
            DOVirtual.DelayedCall(startDelay, () =>
            {
                Destroy(heroInTitle.gameObject);
                this.gameObject.SetActive(false);
                notOnCanvas.SetActive(false);
                startGame.OnNext(Unit.Default);
            });
        });
        rankingButton.OnClickAsObservable().Subscribe(_ =>
        {
            blind.enabled = true;
            blind.Play("eyeEffect_UI_blink", 0, 0);
            DOVirtual.DelayedCall(startDelay, () =>
            {
                this.gameObject.SetActive(false);
                notOnCanvas.SetActive(false);
                
                ranking.Enter();
                ranking.IsActive.Where(active => !active).Take(1).Subscribe(__ =>
                {
                    this.gameObject.SetActive(true);
                    notOnCanvas.SetActive(true);
                });
            });
        });
        StartGame = startGame;

        ranking.IsActive.Subscribe(active =>
        {
            heroInTitle.AcceptsInput = !active;
        });
    }
}