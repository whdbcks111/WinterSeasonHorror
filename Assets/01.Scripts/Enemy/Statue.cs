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
        // ���� ĳ��Ʈ ����
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
                //�÷��̾ �þ߾ȿ� ��� ����� �۵��� ����. ���ο� ���¸� ���� ��. Ordered ����.
            }
        }
    }
}
