using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundDatabase", menuName = "Database/SoundDatabase", order = 0)]
public class SoundDatabase : ScriptableObject
{
    public AudioClip heroRun;
    public AudioClip heroAttack;
    public AudioClip heroAttackHit1;
    public AudioClip heroAttackHit2;
    public AudioClip heroAttackHit3;
    public AudioClip heroDamage;
    public AudioClip heroDie;
    public AudioClip zakoDamage;
    public AudioClip scoreUp;
    public AudioClip buttonPushed;

    [Space(10)] public AudioClip bgmMain;
    
    public static SoundDatabase Instance { get; private set; }
    
    public SoundDatabase()
    {
        Instance = this;
    }
}