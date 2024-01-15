[System.Serializable]
public class PlayerData
{
    public float[] position;
    public float light ;
    public float stamina;
    // 추가적으로 저장할 변수들을 선언합니다.

    public PlayerData(Player player)
    {
        light = player.LightEnerge;
        var playerPos = player.transform.position;
        stamina = player.Stamina;
        position = new float[3];
        position[0] = playerPos.x;
        position[1] = playerPos.y;
        position[2] = playerPos.z;
        
        //health = player.health;
        // 추가적으로 필요한 변수들을 저장합니다.
    }
}


