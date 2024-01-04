using UnityEngine;
using System.Collections.Generic;
using Febucci.UI;
using TMPro;

[CreateAssetMenu(fileName = "ChatScript", menuName = "ScriptableObjects/ChatScript", order = 1)]
public class ChatScript : ScriptableObject
{
    public bool isStartConv = false;  //대화를 먼저 시작하는 불리언 변수 추천
    public TextAnimatorPlayer tanimPlayer;
    
    public TextAnimator example;
    // TextMeshPro 객체들을 저장하는 리스트
    [TextArea(1,3)]
    public string name;
    public int chapter;
    public int chatCount;
    
    [TextArea(1,3)]
    public List<string> textList = new List<string>();
    public List<TextMeshPro> tmproList = new List<TextMeshPro>();
}

