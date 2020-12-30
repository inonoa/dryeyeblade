
using DG.Tweening;
using UnityEngine;

public class CameraFollowsHero : MonoBehaviour
{
    [SerializeField] Vector2 offsetInTitle = Vector2.right;
    [SerializeField] float cameraShakeStrength = 1;
    [SerializeField] float cameraShakeDuration = 0.5f;
    [SerializeField] int cameraShakeVibrato = 10;
    
    Vector2 shakeOffset = Vector2.zero;

    bool shaked = false;
    void LateUpdate()
    {
        if (shaked) return;
        
        if (Hero.Current.Value is null)
        {
            transform.position = new Vector3(0, 0, -10);
            return;
        }

        Vector3 heroPos = Hero.Current.Value.transform.position;
        
        transform.position = Hero.Current.Value.InTitle ?
              new Vector3(heroPos.x + offsetInTitle.x, heroPos.y + offsetInTitle.y, -10) 
            : new Vector3(heroPos.x, heroPos.y, -10) + shakeOffset.Vec3();
    }

    public void Shake()
    {
        DOTween.Shake
        (
            () => shakeOffset,
            val => shakeOffset = val,
            cameraShakeDuration, cameraShakeStrength, cameraShakeVibrato
        );
    }
}