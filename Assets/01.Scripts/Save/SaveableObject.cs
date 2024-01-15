

using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class SaveableObject : MonoBehaviour
{
    public string UUID = null;

    private void Awake()
    {
        UUID ??= GUID.Generate().ToString();
    }

    public void GenerateUniqueID()
    {
        //UUID ??= GUID.Generate().ToString();
        UUID ??= GUID.Generate().ToString();
        if (UUID.Equals(""))
        {
            UUID = GUID.Generate().ToString();
        }
        //Debug.Log("InstanceID: " + UUID);
    }

    

}