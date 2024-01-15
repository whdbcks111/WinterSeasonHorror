using System;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.VisualScripting;
[ExecuteAlways]
public class SaveManager : MonoBehaviour
{
    private Player player;
    private static SaveableObject[] _saveableObjects;
    
    

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveGameData();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadGameData();
        }
        
    }

    public void SaveGame()
    {
        SavePlayer(player);
    }

    public void LoadGame()
    {
        /*PlayerData data = LoadData();
        if (data != null)
        {
            LoadPlayer(data);
        }*/
    }

    private void Awake()
    {
        
    }

    private void Start()
    {
        //_saveableObjects = Resources.FindObjectsOfTypeAll<SaveableObject>();
        _saveableObjects = Resources.FindObjectsOfTypeAll<SaveableObject>();
        foreach (var a in _saveableObjects )
        {
            var temp = a.gameObject.activeSelf;
            if(!temp) a.gameObject.SetActive(true);
            a.GenerateUniqueID();
            Debug.Log("result" + " " + a.name + " " + a.UUID);
            if(!temp) a.gameObject.SetActive(false);
            
        }
        player = Player.Instance;
    }

    public static PlayerData SavePlayer(Player player)
    {
        PlayerData playerData = new PlayerData(player);
        return playerData;
    }
    public static void SaveGameData()
    {
        var formatter = new BinaryFormatter();
        var path = Application.persistentDataPath + "/GameData.save";
        var stream = new FileStream(path, FileMode.Create);
        
        var playerData = SavePlayer(Player.Instance);
        var targetData = SaveTargetObjects();
        var interactData = SaveInteractableObjects();

        var gameData = new GameData(interactData, playerData,targetData);
        formatter.Serialize(stream, gameData);
        stream.Close();
    }

    public static void LoadGameData()
    {
        GameData data = LoadData();
        LoadTargetObjects(data.TargetObjectsData);
        LoadInteractableObjects(data.InterObjData);
        LoadPlayer(data.PlayerData);
    }
    public static void LoadTargetObjects(TargetObjectData[] data)
    {
        var saveableObjects =_saveableObjects;

        foreach (var dataItem in data)
        {
            foreach (var saveableObject in saveableObjects)
            {
                if (saveableObject.UUID == dataItem.uniqeID)
                {
                    var position = new Vector3(dataItem.position[0], dataItem.position[1],
                        dataItem.position[2]);
                    saveableObject.transform.position = position;
                    saveableObject.gameObject.SetActive(dataItem.isActive);
                    break; // UUID가 일치하는 오브젝트를 찾았으므로 내부 루프를 종료합니다.
                }
            }
        }
    }
    
  

    public static TargetObjectData[] SaveTargetObjects()
    {
        
        var targetObjs = _saveableObjects;
        TargetObjectData[] targetObjectDatas = new TargetObjectData[targetObjs.Length];
        for (int i = 0; i < targetObjs.Length; i++)
        {
            targetObjectDatas[i] = new TargetObjectData(targetObjs[i].gameObject);
        }

        return targetObjectDatas;
    }
    public static void LoadInteractableObjects(InteractableObjectData[] data)
    {
        var interactableObjects = Resources.FindObjectsOfTypeAll<InteractableObject>();

        foreach (var dataItem in data)
        {
            foreach (var interactrObject in interactableObjects)
            {
                if (interactrObject.GetComponent<SaveableObject>().UUID == dataItem.uniqueId)
                {
                    
                    var position = new Vector3(dataItem.position[0], dataItem.position[1],
                        dataItem.position[2]);
                    interactrObject.transform.position = position;
                    interactrObject.gameObject.SetActive(dataItem.isActive);
                    interactrObject.isOn = dataItem.isOn;
                    interactrObject.ProvideVisualFeedback(dataItem.isOn);
                    break; // UUID가 일치하는 오브젝트를 찾았으므로 내부 루프를 종료합니다.
                }
            }
        }
    }
    public static InteractableObjectData[] SaveInteractableObjects()
    {
        
        var interactableObjects = FindObjectsOfType<InteractableObject>();
        InteractableObjectData[] interactableObjectDatas = new InteractableObjectData[interactableObjects.Length];
        for (int i = 0; i < interactableObjects.Length; i++)
        {
            interactableObjectDatas[i] = new InteractableObjectData(interactableObjects[i]);
        }

        return interactableObjectDatas;
    }
    
    public static void LoadPlayer(PlayerData data)
    {
        var position = new Vector3(data.position[0], data.position[1], data.position[2]);
        // 추가적으로 필요한 데이터를 로드합니다.
        Player.Instance.LightEnerge = data.light;
        Player.Instance.transform.position = position; // 플레이어의 위치를 업데이트합니다.
        Player.Instance.Stamina = data.stamina;
    }

    public static GameData LoadData()
    {
        string path = Application.persistentDataPath + "/GameData.save";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            GameData data = formatter.Deserialize(stream) as GameData;
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
}