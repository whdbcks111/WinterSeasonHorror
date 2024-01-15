using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public InteractableObjectData[] InterObjData;
    public PlayerData PlayerData;
    public TargetObjectData[] TargetObjectsData;

    public GameData(InteractableObjectData[] interObjDatas, PlayerData playerData, TargetObjectData[] targetObjectDatas )
    {
        InterObjData = interObjDatas;
        PlayerData = playerData;
        TargetObjectsData = targetObjectDatas;
    }
}
