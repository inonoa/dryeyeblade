using System;
using System.Linq;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class HeroEyeView : MonoBehaviour
{
    [SerializeField] Animator blindAnim;
    [SerializeField] PostProcessVolume postProcess;
    [SerializeField] float bloomIntensityInClosing = 20;
    [SerializeField] float bloomIntensityInOpening = 10;
    
    Tween openOrCloseCurrent;
    Bloom bloom;

    void Awake()
    {
        bloom = postProcess.profile.settings.First(setting => setting is Bloom) as Bloom;
        bloom.intensity.value = bloomIntensityInOpening;
        
        Hero.Current
            .Where(hero => hero != null)
            .Subscribe(hero =>
        {
            hero.Eye.IsOpen.SkipLatestValueOnSubscribe()
                .Subscribe(open =>
                {
                    blindAnim.enabled = true;
                    blindAnim.Play(open ? "eyeEffect_open" : "eyeEffect_close");
                    openOrCloseCurrent?.Kill();
                    openOrCloseCurrent = DOVirtual.DelayedCall
                    (
                        blindAnim.GetCurrentAnimatorStateInfo(0).length,
                        () => blindAnim.enabled = false
                    );

                    DOTween.To
                    (
                        ()  => bloom.intensity.value,
                        val => bloom.intensity.value = val,
                        open ? bloomIntensityInOpening : bloomIntensityInClosing,
                        blindAnim.GetCurrentAnimatorStateInfo(0).length / 3
                    );
                })
                .AddTo(this);
        });
    }
}