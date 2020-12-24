using System;
using DG.Tweening;
using UniRx;
using UnityEngine;

public class HeroAttackView : MonoBehaviour
{
    [SerializeField] HeroAttack attack;
    [SerializeField] Animator effectAnimPrefab;
    [SerializeField] float effectPositionOffset = 0.3f;

    void Start()
    {
        attack.OnAttack
            .Subscribe(hero =>
            {
                var effect = Instantiate
                (
                    effectAnimPrefab,
                    transform.position + effectPositionOffset * hero.EyeDirection.Value.ToVec2().Vec3(),
                    Quaternion.identity
                );
                effect.Play("attackEffect_" + hero.EyeDirection.Value.ToSimpleStr());
                DOVirtual.DelayedCall(1, () => Destroy(effect.gameObject));
            });
    }
}