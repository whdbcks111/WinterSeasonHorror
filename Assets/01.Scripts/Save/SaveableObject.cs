

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
[ExecuteAlways]
public class SaveableObject : MonoBehaviour
{
    public string UUID = null;


#if UNITY_EDITOR
    private void Awake()
    {
        UUID ??= GUID.Generate().ToString();
    }

    public void GenerateUniqueID()
    {
        
        //UUID ??= GUID.Generate().ToString();
        UUID = GUID.Generate().ToString();
        //Debug.Log("InstanceID: " + UUID);
    }
#endif


}