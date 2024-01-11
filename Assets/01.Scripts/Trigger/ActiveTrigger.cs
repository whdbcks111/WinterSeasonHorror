

using System;
using UnityEngine;

public class ActiveTrigger : BaseTrigger
{
    [Header("��Ƽ�� ��� ������Ʈ��")]
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