using System;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class HeroEyeView : MonoBehaviour
{
    HeroEye eye;
    [SerializeField] Animator blindAnim;
    [SerializeField] Slider eyeSlider;
    [SerializeField] HeroParams param;

    void Start()
    {
        Hero.CurrentSet.Subscribe(hero =>
        {
            if(hero == null) return;
            eye = hero.Eye;
            hero.Eye.IsOpen.Skip(1)
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
                })
                .AddTo(this);
        });
    }

    Tween openOrCloseCurrent;

    void Update()
    {
        if(eye is null) return;
        if(!eye.gameObject.activeInHierarchy) return;
        
        if (eye.IsOpen.Value)
        {
            eyeSlider.value = Mathf.Clamp01(eye.SecondsFromOpen / param.CoolTime);
        }
        else
        {
            eyeSlider.value = 1 - Mathf.Clamp01(eye.SecondsFromClose / param.EyeClosedTimeMax);
        }
    }
}