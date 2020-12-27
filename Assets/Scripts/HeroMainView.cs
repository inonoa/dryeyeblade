using System;
using System.Collections;
using System.Linq;
using UniRx;
using UnityEngine;

public class HeroMainView : MonoBehaviour
{
    [SerializeField] Hero hero;
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] new AudioSource audio;

    Dir8 lastEyeDir = Dir8.D;

    void Start()
    {
        hero.State.Subscribe(state =>
        {
            switch (state)
            {
                case Hero.EState.Normal:
                    if (hero.KeyDirection.Value == Dir8.None)
                    {
                        animator.Play(DirToStateIdle(lastEyeDir));
                    }
                    else
                    {
                        animator.Play(DirToStateRun(hero.KeyDirection.Value));
                        lastEyeDir = hero.KeyDirection.Value;
                    }
                    break;
            }
        })
        .AddTo(this);
        
        hero.KeyDirection
            .Where(_ => hero.State.Value == Hero.EState.Normal)
            .Subscribe(dir =>
            {
                if (dir == Dir8.None)
                {
                    animator.Play(DirToStateIdle(lastEyeDir));
                }
                else
                {
                    animator.Play(DirToStateRun(dir));
                    lastEyeDir = dir;
                }
            })
            .AddTo(this);
        
        hero.OnDamaged.Subscribe(_ =>
        {
            animator.Play(DirToStateDamaged(hero.EyeDirection.Value));
            StartCoroutine(DamageBlink());
            audio.PlayOneShot(SoundDatabase.Instance.heroDamage);
        })
        .AddTo(this);

        hero.OnDeath.Subscribe(_ =>
        {
            audio.PlayOneShot(SoundDatabase.Instance.heroDie);
        });
    }

    IEnumerator DamageBlink()
    {
        int blinkTimes = 3;
        foreach (int _ in Enumerable.Range(0, blinkTimes))
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(hero.Param.StunTime / blinkTimes / 2);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(hero.Param.StunTime / blinkTimes / 2);
        }
    }

    static string Dir8ToDir4Str(Dir8 dir8)
    {
        switch (dir8)
        {
            case Dir8.R:  return "r";
            case Dir8.RU: return "u";
            case Dir8.U:  return "u";
            case Dir8.LU: return "u";
            case Dir8.L:  return "l";
            case Dir8.LD: return "d";
            case Dir8.D:  return "d";
            case Dir8.RD: return "d";
            default: throw new ArgumentException();
        }
    }

    static string DirToStateIdle(Dir8 dir)
    {
        return dir == Dir8.None ? "hero_d" : "hero_" + Dir8ToDir4Str(dir);
    }

    static string DirToStateRun(Dir8 dir)     => "hero_run_"    + Dir8ToDir4Str(dir);
    static string DirToStateDamaged(Dir8 dir) => "hero_damage_" + Dir8ToDir4Str(dir);
}