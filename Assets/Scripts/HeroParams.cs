using UnityEngine;

[CreateAssetMenu(fileName = "HeroParams", menuName = "Database/HeroParams", order = 0)]
public class HeroParams : ScriptableObject
{
    [SerializeField] float _SpeedMax;
    [SerializeField] float _RunForce;
    [SerializeField] float _Resistance;
    [SerializeField] KeyCode _EyeKey;
    [SerializeField] KeyCode _AttackKey;
    [SerializeField] float _NormalDamage;
    [SerializeField] float _CoolTime;
    [SerializeField] float _EyeClosedTimeMax;
    [SerializeField] float _StunTime;
    [SerializeField] float _KnockBackDistance;

    public float SpeedMax   => _SpeedMax;
    public float RunForce   => _RunForce;
    public float Resistance => _Resistance;
    public KeyCode EyeKey => _EyeKey;
    public KeyCode AttackKey => _AttackKey;
    public float NormalDamage => _NormalDamage;
    public float CoolTime => _CoolTime;
    public float EyeClosedTimeMax => _EyeClosedTimeMax;
    public float StunTime => _StunTime;
    public float KnockBackDistance => _KnockBackDistance;
    
#if UNITY_EDITOR
    
    [Space(10)]
    [SerializeField] bool _DebugMuteki = false;

    public bool DebugMuteki => _DebugMuteki;
    
#endif
}