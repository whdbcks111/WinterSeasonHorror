using UnityEngine;

[System.Serializable]
public class TargetObjectData
{
    public float[] position;
    public bool isActive;
    public string uniqeID;

    public TargetObjectData(GameObject obj)
    {
        var objPos = obj.transform.position;
        uniqeID = obj.GetComponent<SaveableObject>().UUID;
        position = new float[3];
        position[0] = objPos.x;
        position[1] = objPos.y;
        position[2] = objPos.z;
        isActive = obj.activeSelf;
        //name = obj.name;
    }
}