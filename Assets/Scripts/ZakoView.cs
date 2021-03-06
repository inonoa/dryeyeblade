﻿using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

public class ZakoView : MonoBehaviour, IDoOnTimeStopped
{
    [SerializeField] Zako zako;
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Animator effectPrefab;
    [SerializeField] new AudioSource audio;

    void Start()
    {
        zako.WanderDirSet
            .SkipLatestValueOnSubscribe()
            .Where(dir => zako.State.Value == Zako.EState.Wandering)
            .Subscribe(dir => animator.Play("slime_" + DirToStateStr(dir)));

        zako.OnDamaged.Subscribe(_ =>
        {
            DOVirtual.DelayedCall(Random.Range(0, 0.2f), () => audio.PlayOneShot(SoundDatabase.Instance.zakoDamage));
            StartCoroutine(Blink(onEnd: () => zako.OnDamageDisplayEnded()));
        });

        zako.State.Subscribe(state =>
        {
            switch (state)
            {
                case Zako.EState.Wandering:
                    animator.Play("slime_" + DirToStateStr(zako.WanderDir));
                    break;
                case Zako.EState.ChaseHero:
                    animator.Play("slime_run_" + DirToStateStr(zako.RunDir.ToDir8()));
                    break;
                case Zako.EState.Attack:
                    animator.Play("slime_attack_" + DirToStateStr(zako.RunDir.ToDir8()));
                    DOVirtual.DelayedCall(1.6f, () =>
                    {
                        if(zako.State.Value != Zako.EState.Attack) return;
                        
                        var effect = Instantiate(effectPrefab, this.transform.position, Quaternion.identity);
                        onTimeStopped.Subscribe(_ => effect.DOPause()).AddTo(effect);
                        onTimeRestarted.Subscribe(_ => effect.DOTogglePause()).AddTo(effect);
                        DOVirtual.DelayedCall(2f, () => Destroy(effect.gameObject));
                    })
                    .ReactsToHeroEye();
                    break;
                case Zako.EState.Dead:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        });
    }

    Dir8 chaseDirLast = Dir8.None;
    void Update()
    {
        if(zako.State.Value != Zako.EState.ChaseHero) return;

        var runDir = zako.RunDir.ToDir8();
        if (runDir != chaseDirLast)
        {
            chaseDirLast = runDir;
            animator.Play("slime_run_" + DirToStateStr(runDir));
        }
    }
    
    IEnumerator Blink(Action onEnd)
    {
        Dir8 dir_8 = zako.RunDir.ToDir8(); //RunDir??
        bool dir_l = dir_8.IsL() || dir_8 == Dir8.D;
        animator.Play(dir_l ? "slime_damage_l" : "slime_damage_r");
        foreach (var _ in Enumerable.Range(0, 3))
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(0.13f);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(0.13f);
        }
        onEnd.Invoke();
    }

    Subject<Unit> onTimeStopped = new Subject<Unit>();
    public void OnTimeStopped()
    {
        animator.enabled = false;
        onTimeStopped.OnNext(Unit.Default);
    }

    Subject<Unit> onTimeRestarted = new Subject<Unit>();
    public void OnTimeRestarted()
    {
        animator.enabled = true;
        onTimeRestarted.OnNext(Unit.Default);
    }

    static string DirToStateStr(Dir8 dir)
    {
        switch (dir)
        {
            case Dir8.R:  return "rd";
            case Dir8.RU: return "ru";
            case Dir8.U:  return "ru";
            case Dir8.LU: return "lu"; 
            case Dir8.L:  return "ld"; 
            case Dir8.LD: return "ld";
            case Dir8.D:  return "ld";
            case Dir8.RD: return "rd"; 
            default: throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
        }
    }
}