using UnityEngine;
using System;
using System.Collections.Generic;
using Febucci.UI;
using TMPro;

[CreateAssetMenu(fileName = "ChatScript", menuName = "ScriptableObjects/ChatScript", order = 1)]
public class ChatScript : ScriptableObject
{

    // TextMeshPro 객체들을 저장하는 리스트
    
    public int chapter;
    public int chatCount;
    public List<SpeakerType> participants = new List<SpeakerType>(); // 대화 참여자들의 이름 리스트
    
    public List<DialogueEntry> dialogues = new List<DialogueEntry>();
    // 대화 리스트

    
}
[System.Serializable]
public struct DialogueEntry
{
    public SpeakerType speaker;
    [TextArea(0, 3)]
    public string dialogue; // 대화 내용
}

public enum SpeakerType
{
    Player = 0,
    NPC1 =1,
    NPC2 =2
 
}

