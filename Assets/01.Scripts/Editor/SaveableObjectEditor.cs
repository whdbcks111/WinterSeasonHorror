using UnityEngine;
using UnityEditor;

public class SaveableObjectEditor : EditorWindow
{
    [MenuItem("Tools/Mark Saveable Objects")]
    public static void MarkSaveableObjects()
    {
        int layer = LayerMask.NameToLayer("Pushable");
        
        foreach (Lever lever in FindObjectsOfType<Lever>())
        {
            foreach (GameObject targetObject in lever.targetObjects)
            {
                if(targetObject)
                AddSavableComponent(targetObject);
                

            }
        }
        
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            
           
            if (obj.layer == layer)
            {
                AddSavableComponent(obj);
                continue;
            }
            var trigger = obj.GetComponent<DetectAreaTrigger>();
            var Enemy =  obj.GetComponent<Believer>();
            var player = obj.GetComponent<Player>();
            if (player)
            {
                AddSavableComponent(obj);
            }
            if(Enemy) {
                AddSavableComponent(obj);
                continue;
            }
            var myScript = obj.GetComponent<InteractableObject>(); // 'MyScript'를 여러분의 스크립트 이름으로 대체하세요.
            if (trigger != null && trigger.TriggerOnce) // 'MyBooleanVariable'을 해당 변수 이름으로 대체하세요.
            {
                AddSavableComponent(obj);
                continue;
            }

            if (myScript)
            {
                AddSavableComponent(obj);
            }
            
        }
    }

    public static void AddSavableComponent(GameObject obj)
    {
        Debug.Log("Add Saveable Object In Editor!! " + " Object Name: " +obj.name);
        if(!obj.GetComponent<SaveableObject>())
            obj.AddComponent<SaveableObject>();
    }
}