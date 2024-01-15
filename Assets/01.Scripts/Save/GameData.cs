using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    public InteractableObjectData[] InterObjData;
    public PlayerData PlayerData;
    public EnemyData EnemyData;
    public TargetObjectData[] TargetObjectsData;
    public TriggerData[] TriggerData; // TriggerData 배열 추가

    public GameData(EnemyData enemyData, InteractableObjectData[] interObjData, PlayerData playerData, TargetObjectData[] targetObjectsData, TriggerData[] triggerData)
    {
        EnemyData = enemyData;
        InterObjData = interObjData;
        PlayerData = playerData;
        TargetObjectsData = targetObjectsData;
        TriggerData = triggerData; // 초기화
    }
}

