using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class TitleScene : MonoBehaviour
{
    [SerializeField] Button startButton;
    [SerializeField] GameObject notOnCanvas;
    [SerializeField] Animator blind;
    [SerializeField] float startDelay = 0.5f;
    [SerializeField] Hero heroInTitle;

    public IObservable<Unit> StartGame { get; private set; }

    void Awake()
    {
        var startGame = new Subject<Unit>();
        
        startButton.OnClickAsObservable().Subscribe(_ =>
        {
            blind.enabled = true;
            blind.Play("eyeEffect_UI_blink");
            DOVirtual.DelayedCall(startDelay, () =>
            {
                Destroy(heroInTitle.gameObject);
                this.gameObject.SetActive(false);
                notOnCanvas.SetActive(false);
                startGame.OnNext(Unit.Default);
            });
        });
        StartGame = startGame;
    }
}