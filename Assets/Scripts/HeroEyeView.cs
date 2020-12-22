using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class HeroEyeView : MonoBehaviour
{
    HeroEye eye;
    [SerializeField] Image blind;
    [SerializeField] Slider eyeSlider;
    [SerializeField] HeroParams param;

    void Start()
    {
        Hero.CurrentSet.Subscribe(hero =>
        {
            eye = hero.Eye;
            hero.Eye.IsOpenChanged
                .Subscribe(open => { blind.enabled = !open; })
                .AddTo(this);
        });
    }

    void Update()
    {
        if(eye is null) return;
        if(!eye.gameObject.activeInHierarchy) return;
        
        if (eye.IsOpen)
        {
            eyeSlider.value = Mathf.Clamp01(eye.SecondsFromOpen / param.CoolTime);
        }
        else
        {
            eyeSlider.value = 1 - Mathf.Clamp01(eye.SecondsFromClose / param.EyeClosedTimeMax);
        }
    }
}