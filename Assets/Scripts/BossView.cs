using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

public class BossView : MonoBehaviour, IDoOnTimeStopped
{
    [SerializeField] Animator animator;
    [SerializeField] Animator effectAnim;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] new AudioSource audio;

    [SerializeField] Boss boss;

    List<Tween> tweens = new List<Tween>();
    IEnumerator blink;
    
    void Start()
    {
        Boss.EState lastState = Boss.EState.Normal;
        boss.State.Subscribe(state =>
        {
            switch (state)
            {
            case Boss.EState.Normal:
                animator.Play("boss_normal", 0, 0);
                break;
            case Boss.EState.Attacking:
                animator.Play("boss_attack");
                tweens.Add(DOVirtual.DelayedCall(1f, () =>
                {
                    effectAnim.enabled = true;
                    effectAnim.Play("BossEffect_default", 0, 0);
                    Camera.main.GetComponent<CameraFollowsHero>().Shake();
                    audio.PlayOneShot(SoundDatabase.Instance.bossAttack);
                }));
                break;
            case Boss.EState.Damaged:
                animator.Play
                (
                    lastState == Boss.EState.Attacking ? "boss_damage_in_attack" : "boss_damage_in_normal",
                    0,
                    0
                );
                blink = Blink();
                StartCoroutine(blink);
                DOVirtual.DelayedCall(Random.Range(0, 0.2f), () => audio.PlayOneShot(SoundDatabase.Instance.zakoDamage));
                break;
            case Boss.EState.Dead:
                animator.Play
                (
                    lastState == Boss.EState.Attacking ? "boss_damage_in_attack" : "boss_damage_in_normal",
                    0,
                    0
                );
                DOVirtual.DelayedCall(Random.Range(0, 0.2f), () => audio.PlayOneShot(SoundDatabase.Instance.zakoDamage));
                break;
            }

            lastState = state;
        });
        
        Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(_ =>
        {
            tweens = tweens.Where(tw => tw.IsActive()).ToList();
        })
        .AddTo(this);
    }
    
    IEnumerator Blink()
    {
        foreach (var _ in Enumerable.Range(0, 3))
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(0.13f);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(0.13f);
        }
    }

    bool effectLastEnabled = false;
    public void OnTimeStopped()
    {
        animator.enabled = false;
        effectLastEnabled = effectAnim.enabled;
        effectAnim.enabled = false;
        tweens.ForEach(tw => tw.Pause());
        if(blink != null) StopCoroutine(blink);
    }

    public void OnTimeRestarted()
    {
        animator.enabled = true;
        effectAnim.enabled = effectLastEnabled;
        tweens.ForEach(tw => tw.TogglePause());
        if(blink != null) StartCoroutine(blink);
    }
}