

using System;
using UnityEngine;

public class SaveTrigger : BaseTrigger
{

    public override void Enter()
    {
        Debug.Log("Save");
        SaveManager.Instance.SaveGameData();
    }

    public override void Exit()
    {
    }
}