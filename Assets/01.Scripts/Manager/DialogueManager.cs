using System.Collections;
using System.Collections.Generic;
using Febucci.UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;

public class DialougManager : MonoBehaviour
{
    
    
    private TextAnimatorPlayer curTextPlayer;
    public Canvas curTextBox;
    public ChatScript chatScript;
    [Header("Player")] 
    public Transform playerFooter;
    public TextMeshProUGUI playerText;
    public int playerChatCount = 0;
    [SerializeField]
    private TextAnimatorPlayer playerTextPlayer;
    
    [Header("NPC")]
    public Transform npcFooter;
    public TextMeshProUGUI npcText;
    public int entityChatCount = 0;
    [SerializeField]
    private TextAnimatorPlayer npcTextPlayer;

    
    public int chatCount = 0;
    
    private bool isTalking = false;
    private bool isSkipable = false;
    private bool flipFlag = false;

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftArrow)) 
        {
            if (!flipFlag)
            {
                FooterFlip(180);
            }
            flipFlag = true;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (flipFlag)
            {
                FooterFlip(-180);
            }
            flipFlag = false;
        }
        if(Input.GetKeyDown(KeyCode.Return))
        {
            if (isTalking)
            {
                SkipDialouge();
            }
            else
            {
                NextProgress();
            }
        }
    }

    public void FooterFlip(float rotationAmout)
    {
        
       // curTextBox.GetComponentInChildren<TextBoxFooterTag>().transform.Rotate(0,rotationAmout,0);
        
    }
    public void SkipDialouge()
    {
        curTextPlayer.SkipTypewriter();
    }
    public void Awake()
    {
        //npcTextPlayer.SkipTypewriter();
    }
    // Start is called before the first frame update
        private void Start()
        {
   
            npcTextPlayer.onTextShowed.AddListener(OnTextShowed);
            playerTextPlayer.onTextShowed.AddListener(OnTextShowed);

            curTextPlayer = chatScript.dialogues[0].speaker == Speaker.Player ? playerTextPlayer : npcTextPlayer;
            curTextPlayer.ShowText(chatScript.dialogues[0].dialogue);
            chatCount++;

        }

        public void StartDialouge()
        {

        }

        public void OnTextShowed()
        {
            isTalking = false;
        }
        public void NextProgress()
        {
            if (chatCount < chatScript.dialogues.Count)
            {
                if(curTextBox)
                curTextBox.enabled = false;
                
                isTalking = true;
                curTextPlayer = chatScript.dialogues[chatCount].speaker == Speaker.Player
                    ? playerTextPlayer
                    : npcTextPlayer;
                curTextBox = curTextPlayer.GetComponentInParent<Canvas>();

                curTextBox.enabled = true;
                curTextPlayer.ShowText(chatScript.dialogues[chatCount].dialogue);
                chatCount++;
            }
            else
            {
                chatCount = 0;
            }
        }
    // Update is called once per frame
   
}
