

using System;
using UnityEngine;

public class PlayerStopTrigger : BaseTrigger
{

    public override void Enter()
    {
        Player.Instance.IsMoveable = false;
    }

    public override void Exit()
    {
        Player.Instance.IsMoveable = true;
    }
}