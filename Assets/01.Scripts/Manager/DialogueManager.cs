using System.Collections;
using System.Collections.Generic;
using Febucci.UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;

public class DialougManager : MonoBehaviour
{
    
    [Header("Player")]
    public ChatScript currentPlayerChatScript;
    public TextMeshProUGUI playerText;
    public int playerChatCount = 0;
    [SerializeField]
    private TextAnimatorPlayer playerTextPlayer;
    
    [Header("NPC")]
    public ChatScript currentNpcChatScript;
    public TextMeshProUGUI npcText;
    public int entityChatCount = 0;
    [SerializeField]
    private TextAnimatorPlayer npcTextPlayer;

  
    public int chatCount = 0;
    public Button btn;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
        
            playerChatCount = currentPlayerChatScript.textList.Count;
            entityChatCount = currentNpcChatScript.textList.Count;
   
    
            btn.onClick.AddListener(NextProgress);

            // 먼저 대화를 시작하는 캐릭터 결정
            if (currentNpcChatScript.isStartConv)
            {
                npcText.text = currentNpcChatScript.textList[0];
                npcTextPlayer.ShowText(npcText.text);
            }
            else
            {
                playerText.text = currentPlayerChatScript.textList[0];
                playerTextPlayer.ShowText(playerText.text);
            }
        

    }

    public void StartDialouge()
    {
        
    }
    public void NextProgress()
    {
        Debug.Log("clickButton");
        chatCount++;

        if (chatCount < playerChatCount + entityChatCount)
        {
            Debug.Log("대화 시작");
            // 현재 대화 순서에 따라 캐릭터의 대화를 표시
            if ((currentNpcChatScript.isStartConv && chatCount % 2 == 0) ||
                (!currentNpcChatScript.isStartConv && chatCount % 2 != 0))
            {
                // NPC의 차례
                Debug.Log("npc차례");
                if (chatCount / 2 < currentNpcChatScript.textList.Count)
                {
                    npcText.text = currentNpcChatScript.textList[chatCount / 2];
                    npcTextPlayer.ShowText(npcText.text);
                }
            }
            else
            {
                Debug.Log("플레이어 차례");
                // 플레이어의 차례
                if (chatCount / 2 < currentPlayerChatScript.textList.Count)
                {
                    playerText.text = currentPlayerChatScript.textList[chatCount / 2];
                    playerTextPlayer.ShowText(playerText.text);
                }
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
