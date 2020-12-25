
using System;
using DG.Tweening;
using UniRx;

public static class StoppableTweenExtension
{
    public static T ReactsToHeroEye<T>(this T tween)
        where T : Tween
    {
        var timeChanger = EnemiesTimeChanger.Current;
        
        IDisposable stop = timeChanger.OnTimeStopped
            .Subscribe(_ => tween.Pause());
        IDisposable restart = timeChanger.OnTimeRestarted
            .Subscribe(_ => tween.TogglePause());

        tween.onComplete += () =>
        {
            stop.Dispose();
            restart.Dispose();
        };
            
        return tween;
    }
}