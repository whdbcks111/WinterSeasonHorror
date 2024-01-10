using System.Collections;
using System.Collections.Generic;
using Febucci.UI;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;
using System.Linq;
using Unity.VisualScripting;

public class DialougManager : MonoBehaviour
{

    [Header("TextBoxMove")]
    public float yOffset = 0;
    public Vector2 targetPos;
    public Vector2 movePos;
    public GameObject targetObject;
    public List<Chatter> participants;


    [Header("TextAnimator")]
    public TextMeshProUGUI tmp;
    public TypewriterByCharacter textPlayer;
    public Canvas textBox;
    public ChatScript chatScript;
    public Transform playerFooter;
    [Range(0, 3f)]
    public float disaapearTextBoxDelay = 0f;
    
 
    public int playerChatCount = 0;
    



    public int chatCount = 0;

    private bool isTalking = false;
    private bool isSkipable = false;
    private bool flipFlag = false;


    public void MoveTextBox( )
    {
        SpriteRenderer sr = targetObject.GetComponent<SpriteRenderer>();
        targetPos = new Vector2 (targetObject.transform.position.x , sr.bounds.max.y);
        if (sr != null)
        {
            playerFooter.transform.rotation = new Quaternion(0,sr.flipX ? 180: 0 ,0,0);
            movePos = targetPos + new Vector2(0, yOffset);
            textBox.transform.position = movePos;
        }
    }
    public void Update()
    {
        if(targetObject)
        MoveTextBox();

        if (Input.GetKeyDown(KeyCode.LeftArrow))
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
        if (Input.GetKeyDown(KeyCode.Return))
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

        //curTextBox.GetComponentInChildren<TextBoxFooterTag>().transform.Rotate(0,rotationAmout,0);

    }
    public void SkipDialouge()
    {
        textPlayer.SkipTypewriter();
    }
 
    public void SetParticipants()
    {
        var p = chatScript.participants;
         foreach(var c in FindObjectsOfType<Chatter>())
        {
            if (p.Contains(c.speakerType))
            {
                participants.Add(c);
            }
        }
    }
    private void OnTextShowed()
    {
        isTalking = false; 
        //textBox.enabled = false;

    }
    // Start is called before the first frame update
    private void Start()
    {
      
        SetParticipants();

        textPlayer.onTextShowed.AddListener(OnTextShowed);
       /* textPlayer.ShowText(chatScript.dialogues[0].dialogue);
        chatCount++;*/
    }

    public void SetTarget(SpeakerType speaker)
    {
        foreach (Chatter chatter in participants)
        {
            chatter.textBoxActive = false;
            if (chatter.speakerType == speaker)
            {
                targetObject = chatter.gameObject;
                chatter.textBoxActive = true;
            }
        }

    }

    public void NextProgress()
    {
        if (chatCount < chatScript.dialogues.Count)
        {
            if (textBox)
                textBox.enabled = true;

            isTalking = true;
            SetTarget(chatScript.dialogues[chatCount].speaker);
            if(targetObject == null)
            {
                Debug.Log("타겟 오브젝트 실패 " + chatScript.dialogues[chatCount].speaker);
            }

            
            MoveTextBox();
            //tmp.text = "";
            
            textPlayer.ShowText(chatScript.dialogues[chatCount].dialogue);
            chatCount++;
        }
        else
        {
            chatCount = 0;
        }
    }
    // Update is called once per frame

}
