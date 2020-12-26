using System;
using UniRx;
using UnityEngine;

public class EnemiesTimeChanger : MonoBehaviour
{
    void Awake()
    {
        Current = this;
    }

    void Start()
    {
        Hero.Current.Subscribe(hero =>
        {
            if(hero == null) return;
            hero.Eye.IsOpen.Where(open => open)
                .Delay(TimeSpan.FromSeconds(hero.Param.EyesOpenToTimeRestart))
                .Subscribe(_ => RestartTime());
            hero.Eye.IsOpen.Where(open => !open)
                .Subscribe(_ => StopTime());
        });
    }

    Subject<Unit> _OnTimeStopped = new Subject<Unit>();
    public IObservable<Unit> OnTimeStopped => _OnTimeStopped;
    void StopTime()
    {
        foreach (var doOnTimeStopped in GetComponentsInChildren<IDoOnTimeStopped>())
        {
            doOnTimeStopped.OnTimeStopped();
        }
        _OnTimeStopped.OnNext(Unit.Default);
    }

    Subject<Unit> _OnTimeRestarted = new Subject<Unit>();
    public IObservable<Unit> OnTimeRestarted => _OnTimeRestarted;
    void RestartTime()
    {
        foreach (var doOnTimeStopped in GetComponentsInChildren<IDoOnTimeStopped>())
        {
            doOnTimeStopped.OnTimeRestarted();
        }
        _OnTimeRestarted.OnNext(Unit.Default);
    }
    
    public static EnemiesTimeChanger Current { get; private set; }
}

public interface IDoOnTimeStopped
{
    void OnTimeStopped();
    void OnTimeRestarted();
}
