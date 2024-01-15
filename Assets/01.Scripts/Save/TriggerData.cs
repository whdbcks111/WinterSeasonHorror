using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TriggerData
{
    public string uniqueId;
    public int enterCount;

    public TriggerData(DetectAreaTrigger trigger)
    {
        uniqueId = trigger.GetComponent<SaveableObject>().UUID;
        enterCount = trigger.EnterCount;
    }
    
    
}
