﻿using System;
using UniRx;
using UnityEngine;

public class HeroEye : MonoBehaviour
{
    [SerializeField] HeroParams param;
    
    ReactiveProperty<bool> _IsOpen = new ReactiveProperty<bool>(true);
    public IReadOnlyReactiveProperty<bool> IsOpen => _IsOpen;

    public float SecondsFromOpen{ get; private set; } = 100;
    public float SecondsFromClose{ get; private set; } = 0;

    void Update()
    {
        if(IsOpen.Value) UpdateOpen();
        else             UpdateClosed();
    }

    void UpdateOpen()
    {
        SecondsFromOpen += Time.deltaTime;

        if (Input.GetKeyDown(param.EyeKey))
        {
            if(SecondsFromOpen < param.CoolTime) return;
            
            Close();
        }
    }
    void UpdateClosed()
    {
        SecondsFromClose += Time.deltaTime;

        if (SecondsFromClose >= param.EyeClosedTimeMax)
        {
            Open();
            return;
        }

        if(Input.GetKeyDown(param.EyeKey)) Open();
    }

    void Close()
    {
        _IsOpen.Value = false;
        SecondsFromClose = 0;
    }

    void Open()
    {
        _IsOpen.Value = true;
        SecondsFromOpen = 0;
    }
}