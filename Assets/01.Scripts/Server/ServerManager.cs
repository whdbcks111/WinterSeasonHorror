using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using BestHTTP.JSON;
using BestHTTP.SocketIO;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Socket = BestHTTP.SocketIO3.Socket;
using SocketManager = BestHTTP.SocketIO3.SocketManager;

using SocketOptions = BestHTTP.SocketIO3.SocketOptions;

public class ServerManager : MonoBehaviour
{
    public Dictionary<string, Sprite> spriteDictionary = new Dictionary<string, Sprite>();
    public List<OtherPlayer> otherPlayers = new List<OtherPlayer>();
    public GameObject userPrefab;
    public static ServerManager Instance { get; private set; }
    [SerializeField]
    private string serverAddress = "ws://localhost:3000";
    private SocketManager socketManager;
    public string currentPlayerId;
    public RoomInfo curRoomInfo;
    private UnityEngine.Vector3 tempPos;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            ConnectToServer();
        }
        else
        {
            Destroy(gameObject);
        }
        Application.runInBackground = true;
    }


    private void ConnectToServer()
    {
        // Socket.IO 매니저 설정
        SocketOptions options = new SocketOptions();
        options.AutoConnect = true;

        // 매니저 인스턴스 생성
        socketManager = new SocketManager(new System.Uri(serverAddress), options);

        // 서버 연결
        socketManager.Open();


        socketManager.Socket.On<string>("Connection", OnConnection);
        socketManager.Socket.On<string>("playerTrigger", OnPlayerTrigger);
        socketManager.Socket.On<string>("playerMove", OnPlayerMove);
    }


    

    private void OnPlayerTrigger(string obj)
    {
        throw new NotImplementedException();
    }

    private OtherPlayer FindOtherPlayerInList(string id)
    {
        if (otherPlayers == null) return null;
        if (!(otherPlayers.Count > 0))return null;
        foreach (var player in otherPlayers)
        {
            if (player.userId == id) return player;
        }

        return null;
    }
    private void OnPlayerMove(string data)
    {
        //Debug.Log(data);
        PlayerData playerData = JsonUtility.FromJson<PlayerData>(data);
      
        if (otherPlayers.Count > 0)
        {
            var obj = FindOtherPlayerInList(playerData.playerID);
            if (obj)
                obj.RefreshData(playerData);
        }

    }
    

    private void OnConnection(string playerID)
    {
        
        Player.Instance.userId = playerID;
        currentPlayerId = playerID;
        Debug.Log("Player ID 부여: " + playerID);
    }
    





    public GameObject FindPlayerById(string playerID)
    {
        if (playerID == currentPlayerId) return Player.Instance.gameObject;
        
        var otherPlayers = FindObjectsOfType<OtherPlayer>();
        foreach (var other in otherPlayers)
        {
            var id = other.userId;
            if (id.Equals(playerID))
            {
                return other.gameObject;
            }
        }

        return null;
    }

    private void Start()
    {
        LoadSprites();
    }
    void LoadSprites()
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>("Player");
        foreach (Sprite sprite in sprites)
        {
            spriteDictionary[sprite.name] = sprite;
        }
    }

    public void CreateOtherPlayer(string[] userIds)
    {
        
        foreach (var id in userIds)
        {
            if (!FindPlayerById(id))
            {
                var obj = Instantiate(userPrefab);
                obj.transform.position = Player.Instance.transform.position;
            }
            
        }
    }
    public GameObject CreateOtherPlayer(string id)
    {
        var otherPlayer = FindPlayerById(id);
            if (!otherPlayer)
            {
                var obj = Instantiate(userPrefab);
                obj.GetComponent<OtherPlayer>().userId = id;
                obj.transform.position = Player.Instance.transform.position;
                
                return obj;
            }

            return otherPlayer;
    }
    

 

    private void OnDestroy()
    {
        if (socketManager != null)
        {
            socketManager.Close();
            socketManager = null;
        }
    }



    private void Update()
    {
        Vector3 pos = Player.Instance.transform.position;
        if (Vector3.Distance(pos, tempPos) > 0.1f)
        {
            tempPos = Player.Instance.transform.position;
            PlayerData data = new PlayerData(Player.Instance);
            socketManager.Socket.Emit("playerMove", JsonUtility.ToJson(data));
        }
        else
        {
            /*PlayerData data = new PlayerData(Player.Instance);
            socketManager.Socket.Emit("playerMove", JsonUtility.ToJson(data));*/
        }
        

        
    }
}

[Serializable]
public class TriggerEvent
{
    public string Id;
    public bool isOn;

    public TriggerEvent()
    {
        
    }
}

