using System;
using UnityEngine;

public class EnemyGroup : MonoBehaviour
{
    [SerializeField] float _Difficulty;
    public float Difficulty => _Difficulty;
    void Update()
    {
        if(transform.childCount == 0) Destroy(gameObject);
    }
}