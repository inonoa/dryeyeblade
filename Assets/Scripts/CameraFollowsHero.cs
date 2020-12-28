
using UnityEngine;

public class CameraFollowsHero : MonoBehaviour
{
    [SerializeField] Vector2 offsetInTitle = Vector2.right;
    void LateUpdate()
    {
        if (Hero.Current.Value is null)
        {
            transform.position = new Vector3(0, 0, -10);
            return;
        }

        Vector3 heroPos = Hero.Current.Value.transform.position;
        
        transform.position = Hero.Current.Value.InTitle ?
              new Vector3(heroPos.x + offsetInTitle.x, heroPos.y + offsetInTitle.y, -10) 
            : new Vector3(heroPos.x, heroPos.y, -10);
    }
}