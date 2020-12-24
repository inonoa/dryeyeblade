using System;
using System.Collections;
using System.Linq;
using UniRx;
using UnityEngine;

public class ZakoView : MonoBehaviour, IDoOnTimeStopped
{
    [SerializeField] Zako zako;
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;

    void Start()
    {
        zako.WanderDirSet
            .Where(dir => zako.State == Zako.EState.Wandering)
            .Subscribe(dir =>
            {
                switch (dir)
                {
                    case Dir8.None:
                        break;
                    case Dir8.R:
                        animator.Play("slime_rd");
                        break;
                    case Dir8.RU:
                        animator.Play("slime_ru");
                        break;
                    case Dir8.U:
                        animator.Play("slime_ru");
                        break;
                    case Dir8.LU:
                        animator.Play("slime_lu");
                        break;
                    case Dir8.L:
                        animator.Play("slime_ld");
                        break;
                    case Dir8.LD:
                        animator.Play("slime_ld");
                        break;
                    case Dir8.D:
                        animator.Play("slime_ld");
                        break;
                    case Dir8.RD:
                        animator.Play("slime_rd");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
                }
            });

        zako.OnDamaged.Subscribe(_ => StartCoroutine(Blink(onEnd: () => zako.OnDamageDisplayEnded())));
    }

    Dir8 chaseDirLast = Dir8.None;
    void Update()
    {
        if(zako.State != Zako.EState.ChaseHero) return;

        var runDir = zako.RunDir.ToDir8();
        if (runDir != chaseDirLast)
        {
            chaseDirLast = runDir;
            switch (runDir)
            {
                case Dir8.None:
                    break;
                case Dir8.R:
                    animator.Play("slime_run_rd");
                    break;
                case Dir8.RU:
                    animator.Play("slime_run_ru");
                    break;
                case Dir8.U:
                    animator.Play("slime_run_ru");
                    break;
                case Dir8.LU:
                    animator.Play("slime_run_lu");
                    break;
                case Dir8.L:
                    animator.Play("slime_run_ld");
                    break;
                case Dir8.LD:
                    animator.Play("slime_run_ld");
                    break;
                case Dir8.D:
                    animator.Play("slime_run_ld");
                    break;
                case Dir8.RD:
                    animator.Play("slime_run_rd");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(runDir), runDir, null);
            }
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

    public void OnTimeStopped()
    {
        animator.enabled = false;
    }

    public void OnTimeRestarted()
    {
        animator.enabled = true;
    }
}