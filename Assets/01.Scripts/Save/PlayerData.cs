using UnityEngine;
using SavedVariables = Unity.VisualScripting.SavedVariables;

[System.Serializable]
public class PlayerData
{
    public string playerID;
    public bool lightActive;
    public bool isFlipX;
    public float light ;
    public float[] position;
    public float stamina;






    //public int spriteNum;
    // 추가적으로 저장할 변수들을 선언합니다.

    public PlayerData(Player player)
    {
        //playerID = player.GetComponent<SaveableObject>().UUID;
        playerID = player.userId;
        lightActive = player.IsLightOn;
        isFlipX = player._spriteRenderer.flipX;
        light = player.LightEnerge;
        var playerPos = player.transform.position;
        stamina = player.Stamina;
        position = new float[3];
        position[0] = playerPos.x;
        position[1] = playerPos.y;
        position[2] = playerPos.z;
        
    }
}


