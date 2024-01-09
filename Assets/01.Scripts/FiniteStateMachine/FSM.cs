using System;
using System.Collections.Generic;

public class FSM
{
    private BaseState _curState = null;

    public BaseState CurrentState
    {
        get => _curState;
        set 
        {
            _curState?.OnExit();
            _curState = value;
            value?.OnEnter();
        }
    }

    public void Update()
    {
        _curState?.OnUpdate();
    }
}