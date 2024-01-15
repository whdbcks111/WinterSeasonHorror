using UnityEngine;

[CreateAssetMenu(fileName = "New JumpScare", menuName = "JumpScare")]
public class JumpScare : ScriptableObject
{
    public Sprite[] sprites; // 스프라이트 배열
    public float[] displayTimes; // 각 스프라이트를 표시할 시간 배열
    public float lastSpriteDuration; // 마지막 스프라이트 표시 시간
    public AudioClip jumpScareSound; // 점프 스케어 사운드
    public UIManager.ScreenFit screenFit;
}