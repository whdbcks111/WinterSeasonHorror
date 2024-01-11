

using System;
using UnityEngine;

public class ChatTrigger : BaseTrigger
{
    public ChatScript ChatScript;

    public override void Enter()
    {
        DialougManager.Instance.StartDialouge(ChatScript);
    }

    public override void Exit()
    {
    }
}