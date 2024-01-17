

using System;
using UnityEngine;

public class BatteryTrigger : BaseTrigger
{

    public override void Enter()
    {
        Player.Instance.LightEnerge = Player.Instance.MaxLightEnerge;
    }

    public override void Exit()
    {
    }
}