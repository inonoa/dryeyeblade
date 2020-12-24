using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ResultScene : MonoBehaviour
{
    [SerializeField] Button restartButton;

    Subject<Unit> _Restart = new Subject<Unit>();

    void Awake()
    {
        restartButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                gameObject.SetActive(false);
                _Restart.OnNext(Unit.Default);
            });
    }
    
    public void Enter()
    {
        gameObject.SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            gameObject.SetActive(false);
            _Restart.OnNext(Unit.Default);
        }
    }

    public IObservable<Unit> Restart => _Restart;
}
