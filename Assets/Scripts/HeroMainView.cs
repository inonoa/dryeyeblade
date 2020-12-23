using System;
using System.Collections;
using UniRx;
using UnityEngine;

public class HeroMainView : MonoBehaviour
{
    [SerializeField] Hero hero;
    [SerializeField] Animator animator;

    Dir8 lastDir = Dir8.D;

    void Start()
    {
        hero.KeyDirectionSet
            .Subscribe(dir =>
            {
                if (dir == Dir8.None)
                {
                    animator.Play(DirToStateIdle(lastDir));
                }
                else
                {
                    animator.Play(DirToStateRun(dir));
                }
                lastDir = dir;
            });
    }

    static string DirToStateIdle(Dir8 dir)
    {
        switch (dir)
        {
            case Dir8.R:  return "hero_r";
            case Dir8.RU: return "hero_u";
            case Dir8.U:  return "hero_u";
            case Dir8.LU: return "hero_u";
            case Dir8.L:  return "hero_l";
            case Dir8.LD: return "hero_d";
            case Dir8.D:  return "hero_d";
            case Dir8.RD: return "hero_d";
            default: throw new ArgumentException();
        }
    }
    static string DirToStateRun(Dir8 dir)
    {
        switch (dir)
        {
            case Dir8.R:  return "hero_run_r";
            case Dir8.RU: return "hero_run_u";
            case Dir8.U:  return "hero_run_u";
            case Dir8.LU: return "hero_run_u";
            case Dir8.L:  return "hero_run_l";
            case Dir8.LD: return "hero_run_d";
            case Dir8.D:  return "hero_run_d";
            case Dir8.RD: return "hero_run_d";
            default: throw new ArgumentException();
        }
    }
}