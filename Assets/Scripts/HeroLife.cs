using System;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

public class HeroLife : MonoBehaviour
{
    [SerializeField] int _LifeMax = 3;
    public int LifeMax => _LifeMax;

    ReactiveProperty<int> _Life;
    public IReadOnlyReactiveProperty<int> Life => _Life;

    [SerializeField] HeroParams param;
    [SerializeField] Hero hero;

    void Awake()
    {
        _Life = new ReactiveProperty<int>(_LifeMax);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(! other.CompareTag("EnemyAttack")) return;
        if(Life.Value == 0) return;
        if(!hero.CanBeDamaged()) return;
        
#if UNITY_EDITOR
        if(param.DebugMuteki) return;
#endif
        
        _Life.Value -= 1;
    }
}