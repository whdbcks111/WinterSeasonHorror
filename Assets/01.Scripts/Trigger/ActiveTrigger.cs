

using System;
using UnityEngine;

public class ActiveTrigger : BaseTrigger
{
    [Header("액티브 대상 오브젝트들")]
    public GameObject[] TargetObjects;

    public override void Enter()
    {
        foreach (var obj in TargetObjects)
        {
            obj.SetActive(true);
        }
    }

    public override void Exit()
    {
        foreach (var obj in TargetObjects)
        {
            obj.SetActive(false);
        }
    }
}