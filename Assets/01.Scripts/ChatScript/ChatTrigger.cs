using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatTrigger : MonoBehaviour
{
    ChatScript chatScript;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Player>())
        {
            DialougManager.Instance.StartDialouge(chatScript);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>())
        {
            DialougManager.Instance.EndDialouge();
        }
    }
}
