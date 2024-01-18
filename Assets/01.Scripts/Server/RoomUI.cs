using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomUI : MonoBehaviour
{
    public TextMeshProUGUI roomId;
    public Button entranceBtn;
    public Button createBtn;
    // Start is called before the first frame update
    void Start()
    {
        createBtn.onClick.AddListener(CreateRoom);
        entranceBtn.onClick.AddListener(EnterRoom);
    }

    private void CreateRoom( )
    {
        ServerManager.Instance.CreateRoom(roomId.text);
    }
    private void EnterRoom( )
    {
        ServerManager.Instance.JoinRoom(roomId.text);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
