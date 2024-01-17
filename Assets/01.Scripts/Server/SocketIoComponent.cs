using BestHTTP.SocketIO3;
using UnityEngine;
using BestHTTP.SocketIO3.Events;
using System;
using BestHTTP.JSON;

public class SocketIoComponent : MonoBehaviour
{
    [SerializeField]
    private string Address = "ws://localhost:3000";
    private SocketManager socketManager = null;
    private bool isConnected = false;
    private void Awake()
    {
        SocketIO3Connect();
    }

    private void Update()
    {
        Test test = new Test()
        {
            x = Player.Instance.transform.position.x,
            y = Player.Instance.transform.position.y,
            z = Player.Instance.transform.position.z
        };
        
        if(isConnected)
         socketManager.Socket.Emit("playerPos", JsonUtility.ToJson(test));
    }

    private void SocketIO3Connect()
    {
        SocketOptions options = new SocketOptions();
        options.AutoConnect = false;

        socketManager = new SocketManager(new System.Uri(Address), options);
        socketManager.Open();
        socketManager.Socket.On<ConnectResponse>(SocketIOEventTypes.Connect, OnConnected);
        
        socketManager.Socket.On("playerPos", (string data) => {
            Debug.Log(data);
        });
    }

    private void OnConnected(ConnectResponse res)
    {
        Debug.Log("Connected! Socket.IO");
        isConnected = true;
        
      
    }
    

    private void PlayerConnected(string data)
    {
        Debug.Log("Player Connected!!");
    }

    private void Destory()
    {
        if (this.socketManager != null)
        {
            this.socketManager.Close();
            this.socketManager = null;
        }
    }

    public void Send(string eventName, string message)
    {
        if (isConnected)
        {
            socketManager.Socket.Emit(eventName, message);
        }
    }
}

[Serializable]
class Test
{
    public float x;
    public float y;
    public float z;
}