using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

public class HeroEye : MonoBehaviour
{
    
    [SerializeField] HeroParams param;
    
    public enum EState{ Open, Closing, Closed, Opening }
    ReactiveProperty<EState> _State = new ReactiveProperty<EState>(EState.Open);
    
    public IReadOnlyReactiveProperty<EState> State => _State;

    IReadOnlyReactiveProperty<bool> _IsOpen;
    public IReadOnlyReactiveProperty<bool> IsOpen
    {
        get
        {
            if (_IsOpen == null)
            {
                _IsOpen = State
                    .Select(state => state == EState.Opening || state == EState.Open)
                    .ToReadOnlyReactiveProperty(true);
            }
            return _IsOpen;
        }
    }
    public bool FullyClosed => State.Value == EState.Closed;

    float secondsFromOpen  = 100;
    float secondsFromClose = 0;

    void Update()
    {
        if(State.Value == EState.Open) UpdateOpen();
        else if(! IsOpen.Value) UpdateClosed();
    }

    void UpdateOpen()
    {
        secondsFromOpen += Time.deltaTime;

        if (Input.GetKeyDown(param.EyeKey))
        {
            if(secondsFromOpen < param.CoolTime) return;
            
            Close();
        }
    }
    void UpdateClosed()
    {
        secondsFromClose += Time.deltaTime;

        if (secondsFromClose >= param.EyeClosedTimeMax)
        {
            Open();
            return;
        }

        if(Input.GetKeyDown(param.EyeKey)) Open();
    }

    void Close()
    {
        _State.Value = EState.Closing;
        secondsFromClose = 0;

        DOVirtual.DelayedCall(0.3f, () => _State.Value = EState.Closed);
    }

    void Open()
    {
        _State.Value = EState.Opening;
        DOVirtual.DelayedCall(param.EyesOpenToTimeRestart, () => _State.Value = EState.Open);
        secondsFromOpen = 0;
    }
}