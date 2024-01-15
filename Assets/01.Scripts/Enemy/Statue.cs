using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Statue : MonoBehaviour
{
    public float radius = 1.0f;
    public float maxDistance = 5.0f;


    Believer _believer;
    Player _player;
    [SerializeField] private Vector2 _boxSize;
    [SerializeField] private Transform _boxPosition;

    //public LayerMask layerMask;
    private void Start()
    {
        _player = Player.Instance;
    }
    void Update()
    {
        // 원형 캐스트 수행
        if(Input.GetKeyDown(KeyCode.V))
        {
            CircleCasting();
        }

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;

        Gizmos.DrawWireCube(_boxPosition.position, _boxSize);
    }
    void CircleCasting()
    {
        RaycastHit2D[] hits = Physics2D.BoxCastAll(_boxPosition.position, _boxSize,0,Vector2.zero);
        foreach(RaycastHit2D hit in hits)
        {
            if(hit.collider.gameObject.TryGetComponent<Believer>(out Believer BelieverScript) && BelieverScript._fsm.CurrentState != BelieverScript._threteningHide)
            {
                BelieverScript._fsm.CurrentState = BelieverScript._chaseState;
                //플레이어가 시야안에 없어서 제대로 작동을 안함. 새로운 상태를 만들 것. Ordered 상태.
            }
        }
    }
}
