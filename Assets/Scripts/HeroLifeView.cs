using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class HeroLifeView : MonoBehaviour
{
    [SerializeField] Animator heart1;
    [SerializeField] Animator heart2;
    [SerializeField] Animator heart3;

    void Start()
    {
        Hero.Current
            .Where(hero => hero != null)
            .DelayFrame(1)
            .Subscribe(hero =>
        {
            hero.Life.Life.Subscribe(val =>
            {
                switch (val)
                {
                case 3:
                    heart1.Play("heart_normal");
                    heart2.Play("heart_normal");
                    heart3.Play("heart_normal");
                    break;
                case 2:
                    heart3.Play("heart_lost");
                    break;
                case 1:
                    heart2.Play("heart_lost");
                    break;
                case 0:
                    heart1.Play("heart_lost");
                    break;
                }
            });
        });
    }
}