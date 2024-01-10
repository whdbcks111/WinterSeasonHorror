using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseState
{
    public Action OnEnter, OnUpdate, OnExit; 

    public BaseState(Action onEnter, Action onUpdate, Action onExit)
    {
        OnEnter = onEnter;
        OnUpdate = onUpdate;
        OnExit = onExit;
    }

}  