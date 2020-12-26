
using UnityEngine;

public class CameraFollowsHero : MonoBehaviour
{
    void LateUpdate()
    {
        if (Hero.Current.Value is null)
        {
            transform.position = new Vector3(0, 0, -10);
            return;
        }

        Vector3 heroPos = Hero.Current.Value.transform.position;
        transform.position = new Vector3(heroPos.x, heroPos.y, -10);
    }
}