using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

public class HeroEyeTimeView : MonoBehaviour
{
    [SerializeField] Hero hero;
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;
    
    [SerializeField, ListDrawerSettings(ShowPaging = false)]
    Sprite[] openSprites;
    [SerializeField, ListDrawerSettings(ShowPaging = false)]
    Sprite[] closeSprites;

    IEnumerator currentCoroutine;
    
    void Start()
    {
        hero.Eye.State.SkipLatestValueOnSubscribe().Subscribe(state =>
        {
            switch (state)
            {
                case HeroEye.EState.Opening:
                    if(currentCoroutine != null) StopCoroutine(currentCoroutine);
                    break;
                case HeroEye.EState.Open:
                    if(currentCoroutine != null) StopCoroutine(currentCoroutine);
                    currentCoroutine = AfterOpen(() =>
                    {
                        animator.enabled = true;
                        animator.Play("eyeGauge_openFinish", 0, 0);
                        float duration = animator.GetCurrentAnimatorStateInfo(0).length;
                        DOVirtual.DelayedCall(duration, () => animator.enabled = false);
                    });
                    StartCoroutine(currentCoroutine);
                break;
                case HeroEye.EState.Closing:
                    animator.enabled = false;
                    if(currentCoroutine != null) StopCoroutine(currentCoroutine);
                    currentCoroutine = AfterClose(() => { });
                    StartCoroutine(currentCoroutine);
                break;
                case HeroEye.EState.Closed:
                break;
            }
        });
    }

    IEnumerator AfterOpen(Action onEnd)
    {
        float secondsPerSprite = hero.Param.CoolTime / openSprites.Length;
        foreach (var sprite in openSprites)
        {
            spriteRenderer.sprite = sprite;
            yield return new WaitForSeconds(secondsPerSprite);
        }
        onEnd.Invoke();
    }

    IEnumerator AfterClose(Action onEnd)
    {
        float secondsPerSprite = hero.Param.EyeClosedTimeMax / closeSprites.Length;
        foreach (var sprite in closeSprites)
        {
            spriteRenderer.sprite = sprite;
            yield return new WaitForSeconds(secondsPerSprite);
        }
        onEnd.Invoke();
    }
}