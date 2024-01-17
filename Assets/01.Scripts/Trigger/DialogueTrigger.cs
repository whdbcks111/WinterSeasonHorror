


using System;
using UnityEngine;

public class DialogueTrigger : BaseTrigger
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