using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayTrigger : BaseTrigger
{
    public DelayedTriggerInfo[] DelayedTriggers;

    public override void Enter()
    {
        EnterTask().Forget();
    }

    private async UniTask EnterTask()
    {
        foreach (var trigger in DelayedTriggers)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(trigger.EnterDelay));
            trigger.Trigger.Enter();
            await UniTask.Delay(TimeSpan.FromSeconds(trigger.ExitDelay));
            trigger.Trigger.Exit();
        }
    }

    public override void Exit()
    {
    }
}

[Serializable]
public class DelayedTriggerInfo
{
    public float EnterDelay = 0f;
    public BaseTrigger Trigger;
    public float ExitDelay = 0f;
}