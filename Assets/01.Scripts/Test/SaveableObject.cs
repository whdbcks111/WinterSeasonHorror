

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

}