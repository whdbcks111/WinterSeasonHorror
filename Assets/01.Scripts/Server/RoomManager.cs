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
public class RoomManager : MonoBehaviour
{
    private SocketManager socketManager;
    public RoomInfo curRoomInfo;
    public static RoomManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Initialize(SocketManager socketManager)
    {
        this.socketManager = socketManager;
        socketManager.Socket.On<string>("RoomCreated", OnRoomCreated);
        socketManager.Socket.On<RoomInfo>("joinedRoom", OnJoinedRoom);
    }

    public void CreateRoom(string roomID)
    {
        var roomData = new RoomInfo(roomID, null);
        roomData.SetGameData(); // 게임 데이터 설정
        socketManager.Socket.Emit("createRoom", JsonUtility.ToJson(roomData));
        curRoomInfo = roomData;
    }

    public void JoinRoom(string roomID)
    {
        socketManager.Socket.Emit("joinRoom", roomID);
    }

    private void OnRoomCreated(string roomID)
    {
        Debug.Log("Room created: " + roomID);
        // 추가적인 방 생성 로직 구현이 필요한 경우 여기에 작성
    }

    private void OnJoinedRoom(RoomInfo roomInfo)
    {
        Debug.Log("Joined room: " + roomInfo.roomID);
        curRoomInfo = roomInfo;
        LoadRoomGameData(roomInfo);
    }

    private void LoadRoomGameData(RoomInfo roomInfo)
    {
        if (roomInfo.gameDataJson != null)
        {
            GameData gameData = roomInfo.GetGameData();
            SaveManager.LoadGameData(gameData);
        }
    }
}
public class RoomInfo
{
    public string roomID;
    public string[] playersID;
    public string gameDataJson;
    public string bossPlayerID;
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
    public GameData GetGameData()
    {
        if (gameDataJson == null) return null;
        
        return JsonUtility.FromJson<GameData>(gameDataJson);
    }
    public void SetGameData()
    {
        gameDataJson = JsonUtility.ToJson(SaveManager.GetGameData());
    }
}