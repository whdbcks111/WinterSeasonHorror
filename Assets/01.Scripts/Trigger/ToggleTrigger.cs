

using System;
using UnityEngine;

public class ToggleTrigger : BaseTrigger
{
    [Header("��Ƽ�� ��� ��� ������Ʈ��")]
    public GameObject[] TargetObjects;

    public override void Enter()
    {
        foreach (var obj in TargetObjects)
        {
            obj.SetActive(!obj.activeSelf);
        }
    }

    public override void Exit()
    {
    }
}