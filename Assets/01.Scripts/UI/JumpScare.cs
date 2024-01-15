using UnityEngine;

[CreateAssetMenu(fileName = "New JumpScare", menuName = "JumpScare")]
public class JumpScare : ScriptableObject
{
    public ScareSpriteEntity[] ScareSpriteEntity;
    public float lastSpriteDuration; // 마지막 스프라이트 표시 시간
    public AudioClip[] jumpScareSounds; // 점프 스케어 사운드
    public UIManager.ScreenFit screenFit;
    
}

[System.Serializable]
public class ScareSpriteEntity
{
    public Sprite sprite;
    public float delayTime;
    
}
