using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Rayex : MonoBehaviour
{
    public float _rayLength;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 레이캐스트를 수행하여 충돌 정보 얻기
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, _rayLength))
        {
            // 충돌 지점에 표시할 동작 수행
            Debug.Log("레이가 " + hit.collider.gameObject.name + "과(와) 충돌했습니다.");

            // 여기서 필요한 추가 동작을 수행할 수 있습니다.
        }

        // 레이 그리기 (디버그용)
        Debug.DrawRay(transform.position, transform.forward * _rayLength, Color.red);
    }
}
