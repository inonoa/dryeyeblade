using System;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class HeroEyeView : MonoBehaviour
{
    [SerializeField] Animator blindAnim;
    
    Tween openOrCloseCurrent;

    void Awake()
    {
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
                })
                .AddTo(this);
        });
    }
}