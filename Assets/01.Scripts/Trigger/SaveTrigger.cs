

using System;
using UnityEngine;

public class SaveTrigger : BaseTrigger
{

    public override void Enter()
    {
        SaveManager.Instance.SaveGameData();
    }

    public override void Exit()
    {
    }
}