using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InteractableObjectData
{
    public bool isActive;
    public float[] position;
    public bool isOn;
    public string uniqueId;
    

    // 추가적으로 저장할 변수들을 선언합니다.

    public InteractableObjectData(InteractableObject obj)
    {
        uniqueId = obj.GetComponent<SaveableObject>().UUID;
        isOn = obj.isOn;
        var objPos = obj.transform.position;
        isActive = obj.gameObject.activeSelf;
        position = new float[3];
        position[0] = objPos.x;
        position[1] = objPos.y;
        position[2] = objPos.z;
        
        //health = player.health;
        // 추가적으로 필요한 변수들을 저장합니다.
    }
}
