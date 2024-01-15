using System;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.VisualScripting;

public class SaveManager : MonoBehaviour
{
    private Player player;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveGame();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadGame();
        }
        
    }

    public void SaveGame()
    {
        SavePlayer(player);
    }

    public void LoadGame()
    {
        PlayerData data = LoadData();
        if (data != null)
        {
            LoadPlayer(data);
        }
    }

    private void Start()
    {
        player = Player.Instance;
    }

    public static void SavePlayer(Player player)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.save";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(player);
        formatter.Serialize(stream, data);
        stream.Close();
    }
    public void LoadPlayer(PlayerData data)
    {
        var position = new Vector3(data.position[0], data.position[1], data.position[2]);
        // 추가적으로 필요한 데이터를 로드합니다.

        player.transform.position = position; // 플레이어의 위치를 업데이트합니다.
        player.Stamina = data.stamina;
    }

    public static PlayerData LoadData()
    {
        string path = Application.persistentDataPath + "/player.save";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            PlayerData data = formatter.Deserialize(stream) as PlayerData;
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