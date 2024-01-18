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

        // 이벤트 리스너 설정
        //socketManager.Socket.On<Socket><BestHTTP.SocketIO.Packet>("RoomCreated",OnRoomCreated);
        socketManager.Socket.On<string>("Connection", OnConnection);
        
        socketManager.Socket.On<string>("RoomCreated", OnRoomCreated);
        socketManager.Socket.On<RoomInfo>("joinedRoom", OnJoinedRoom);
        socketManager.Socket.On<string>("userJoined", OnUserJoined);

        socketManager.Socket.On<string>("playerMove", OnPlayerMove);
        // 여기에 더 많은 이벤트 리스너를 추가할 수 있습니다.
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
        //Debug.Log(playerData.playerID);
        //Debug.Log(playerData.stamina);
        //Debug.Log(JsonUtility.);
        //Debug.Log("playerMove");
        //Debug.Log(data.ToString());
        
        //Debug.Log(JsonUtility.FromJson(data));
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
    
    public void OnUserJoined(string userID)
    {
        Debug.Log("User joined: " + userID);
        if (userID == currentPlayerId) return;

        var existingPlayer = FindOtherPlayerInList(userID);
        if (existingPlayer == null)
        {
            otherPlayers.Add(CreateOtherPlayer(userID).GetComponent<OtherPlayer>());
        }
    }
    public void CreateRoom(string roomID)
    {

        var roomData = new RoomInfo(roomID, null);
        socketManager.Socket.Emit("createRoom",JsonUtility.ToJson(roomData));
        curRoomInfo = roomData;
    }

    public void JoinRoom(string roomID)
    {
        socketManager.Socket.Emit("joinRoom", roomID);
    }

    private void OnRoomCreated(string roomID)
    {
        //socketManager.Socket.Emit("joinRoom", roomID);
        Debug.Log("방 생성 완료!" + roomID);
        
        //string roomID = args[0] as string;
        //Debug.Log("Room Created: " + roomID);
        // 방 생성 로직
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

    private void OnJoinedRoom(RoomInfo roomInfo)
    {
        Debug.Log("Joined room: " + roomInfo.roomID);
        curRoomInfo = roomInfo;

        foreach (var userID in roomInfo.playersID)
        {
            if (userID != currentPlayerId && FindOtherPlayerInList(userID) == null)
            {
                otherPlayers.Add(CreateOtherPlayer(userID).GetComponent<OtherPlayer>());
            }
        }
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
public class RoomInfo
{
    public string roomID;
    public string[] playersID;

    public RoomInfo(string roomID, OtherPlayer[] otherPlayers)
    {
        this.roomID = roomID;
        if (otherPlayers != null)
        {
            playersID = new string[otherPlayers.Length + 1];
            playersID[0] = Player.Instance.userId;
            var count = 1;
            foreach (var other in otherPlayers)
            {
                playersID[count] = other.userId;
                count++;
            }
        }
        else
        {
            playersID = new string[1];
            playersID[0] = Player.Instance.userId;
        }
        
    }
}