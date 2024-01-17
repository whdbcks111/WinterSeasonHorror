using System;
using System.Collections;
using System.Collections.Generic;
using BestHTTP.JSON;
using UnityEngine;
using UnityEngine.UI;
using BestHTTP.SocketIO3;
using BestHTTP.SocketIO.JsonEncoders;


public class SocketTest : MonoBehaviour
{
    SocketIoComponent socket;
    public Button btn;

    public Text id;

    // Start is called before the first frame update
    void Start()
    {
        
        socket = GetComponent<SocketIoComponent>();
 
        
    }

    private void Update()
    {
        /*var pos = Player.Instance.transform.position;

        //if(Player.Instance.mo)
        socket.Send("PlayerMove",Json.Encode(pos));
    }*/
    }

    public void Enter()
    {
        Debug.Log(id.text);
        Join join = new Join();
        join.roomId = id.text;

        socket.Send("playerTrigger", Json.Encode(join));
    }
}

class Join
{
    public string roomId;
}

// public enum RoomEvents
// {
//     JOIN_ROOM = 'joinRoom',
//     LEAVE_ROOM = 'leaveRoom',
//     MOVE_PLAYER = 'movePlayer',
//     MOVE_ITEM = 'moveItem',
// }