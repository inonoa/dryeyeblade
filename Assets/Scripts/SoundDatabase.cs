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
    public AudioClip bossAttack;
    public AudioClip scoreUp;
    public AudioClip buttonPushed;
    public AudioClip dead;

    [Space(10)]
    public AudioClip bgmMain;
    public AudioClip bgmMainLowPassFiltered;
    public AudioClip bgmTitle;
    
    public static SoundDatabase Instance { get; private set; }
    
    public SoundDatabase()
    {
        Instance = this;
    }
}