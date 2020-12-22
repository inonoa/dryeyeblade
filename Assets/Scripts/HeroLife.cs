using System;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

public class HeroLife : MonoBehaviour
{
    [SerializeField] int _LifeMax = 3;
    public int LifeMax => _LifeMax;
    
    ReactiveProperty<int> _Life;
    public IObservable<int> Life => _Life;
    public int LifeValue => _Life.Value;

    void Awake()
    {
        _Life = new ReactiveProperty<int>(_LifeMax);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(! other.CompareTag("EnemyAttack")) return;
        if(LifeValue == 0) return;
        
        _Life.Value -= 1;
    }
}