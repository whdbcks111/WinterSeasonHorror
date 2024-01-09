using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statue : MonoBehaviour
{
    public float radius = 1.0f;
    public float maxDistance = 5.0f;

    Enemy EnemyScript;
    //public LayerMask layerMask;

    void Update()
    {
        // 원형 캐스트 수행
        if(Input.GetKeyDown(KeyCode.E))
        {
            CircleCasting();
        }
    }

    void CircleCasting()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, radius, Vector2.zero, maxDistance);
        foreach (RaycastHit2D hit in hits)
        {
            if(hit.collider.gameObject.TryGetComponent<Enemy>(out EnemyScript))
            {
                StartCoroutine(EnemyScript.EncountWithPlayer());
            }
        }
    }
}
