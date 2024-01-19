

using System;
using UnityEngine;

public class BlindTrigger : BaseTrigger
{

    public override void Enter()
    {
        Player.Instance.IsInBlind = true;
    }

    public override void Exit()
    {
        Player.Instance.IsInBlind = false;
    }
}