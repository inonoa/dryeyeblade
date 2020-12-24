using System;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class TitleScene : MonoBehaviour
{
    [SerializeField] Button startButton;
    [SerializeField] Text titleText;

    public IObservable<Unit> StartGame { get; private set; }

    void Awake()
    {
        var startGame = new Subject<Unit>();
        startButton.OnClickAsObservable().Subscribe(_ =>
        {
            startButton.gameObject.SetActive(false);
            titleText.gameObject.SetActive(false);
            startGame.OnNext(Unit.Default);
        });
        StartGame = startGame;
    }
}