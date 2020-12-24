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
            if(hero == null) return;
            eye = hero.Eye;
            hero.Eye.IsOpen
                .Subscribe(open => { blind.enabled = !open; })
                .AddTo(this);
        });
    }

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