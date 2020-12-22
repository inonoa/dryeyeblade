using System;
using UnityEngine;
using UnityEngine.UI;

public class HeroLifeView : MonoBehaviour
{
    [SerializeField] Text text;

    void Update()
    {
        if(Hero.Current is null) return;
        
        var life = Hero.Current.Life;
        text.text = $"LIFE: {life.LifeValue} / {life.LifeMax}";
    }
}