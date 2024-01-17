

using System;
using UnityEngine;

public class CameraShakeTrigger : BaseTrigger
{
    public float Offset = 0.1f, Time = 0.5f;


    public override void Enter()
    {
        CameraController.Instance.Shake(Offset, Time);
    }

    public override void Exit()
    {
    }
}