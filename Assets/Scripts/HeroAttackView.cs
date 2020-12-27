using System;
using DG.Tweening;
using UniRx;
using UnityEngine;

public class HeroAttackView : MonoBehaviour
{
    [SerializeField] HeroAttack attack;
    [SerializeField] Animator effectAnimPrefab;
    [SerializeField] float effectPositionOffset = 0.3f;
    [SerializeField] new AudioSource audio;

    void Start()
    {
        int numhits = 0;
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
                audio.PlayOneShot(SoundDatabase.Instance.heroAttack, 0.5f);
                DOVirtual.DelayedCall(1, () => Destroy(effect.gameObject));
            });
        attack.Hits.ObserveAdd().Subscribe(addEvt =>
        {
            numhits++;
            switch (numhits)
            {
                case 1:  audio.PlayOneShot(SoundDatabase.Instance.heroAttackHit1); break;
                case 2:  audio.PlayOneShot(SoundDatabase.Instance.heroAttackHit2); break;
                default: audio.PlayOneShot(SoundDatabase.Instance.heroAttackHit3); break;
            }
        });
        attack.KilledEnemies.Subscribe(_ => numhits = 0);
    }
}