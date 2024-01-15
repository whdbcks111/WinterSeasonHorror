[System.Serializable]
public class EnemyData
{
    public float[] position;
    public bool isActive;

    // 추가적으로 저장할 변수들을 선언합니다.

    public EnemyData(Believer enemy)
    {
        var playerPos = enemy.transform.position;
        isActive = enemy.gameObject.activeSelf;
        position = new float[3];
        position[0] = playerPos.x;
        position[1] = playerPos.y;
        position[2] = playerPos.z;
        
        //health = player.health;
        // 추가적으로 필요한 변수들을 저장합니다.
    }
}