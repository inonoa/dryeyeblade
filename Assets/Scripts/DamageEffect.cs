using DG.Tweening;
using UnityEngine;

public class DamageEffect : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Vector2 offset = new Vector2(0.3f, 0.3f);
    [SerializeField] SpriteRenderer[] spriteRenderers;
    [SerializeField] Sprite[] numSprites;
    
    public void Play(Dir8 attackDir)
    {
        if (attackDir == Dir8.R || attackDir == Dir8.RD || attackDir == Dir8.RU)
        {
            animator.transform.position += new Vector3(- offset.x, offset.y);
            animator.Play("damage_l");
        }
        else
        {
            animator.transform.position += offset.Vec3();
            animator.Play("damage_r");
        }

        DOVirtual.DelayedCall(1f, () => Destroy(gameObject));
    }
}