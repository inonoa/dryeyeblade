using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class EnemyGroup : MonoBehaviour
{
    IDamageable[] _Enemies;
    public IReadOnlyList<IDamageable> Enemies
    {
        get
        {
            if (_Enemies == null) _Enemies = GetComponentsInChildren<IDamageable>();
            return _Enemies;
        }
    }
    
    void Update()
    {
        if(transform.childCount == 0) Destroy(gameObject);
    }
}