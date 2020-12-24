using System;
using UniRx;
using UnityEngine;

public class EnemiesTimeChanger : MonoBehaviour
{
    void Start()
    {
        Hero.CurrentSet.Subscribe(hero =>
        {
            if(hero == null) return;
            hero.Eye.IsOpen.Where(open => open)
                .Subscribe(_ => RestartTime());
            hero.Eye.IsOpen.Where(open => !open)
                .Subscribe(_ => StopTime());
        });
    }

    void StopTime()
    {
        foreach (var doOnTimeStopped in GetComponentsInChildren<IDoOnTimeStopped>())
        {
            doOnTimeStopped.OnTimeStopped();
        }
    }

    void RestartTime()
    {
        foreach (var doOnTimeStopped in GetComponentsInChildren<IDoOnTimeStopped>())
        {
            doOnTimeStopped.OnTimeRestarted();
        }
    }
}

public interface IDoOnTimeStopped
{
    void OnTimeStopped();
    void OnTimeRestarted();
}
