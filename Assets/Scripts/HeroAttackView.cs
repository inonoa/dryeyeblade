using System;
using DG.Tweening;
using UniRx;
using UnityEngine;

public class HeroAttackView : MonoBehaviour
{
    [SerializeField] HeroAttack attack;
    [SerializeField] SpriteRenderer spriteRenderer;

    void Start()
    {
        attack.OnAttack
            .Subscribe(_ => spriteRenderer.enabled = true);
        attack.OnAttackFinished
            .Subscribe(_ => spriteRenderer.enabled = false);
    }
}