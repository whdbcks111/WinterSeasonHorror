using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("몬스터 이동 속도")]
    [SerializeField] private float _moveSpeed;
    [Header("몬스터 이동 거리 범위(범위 내 랜덤임)")]
    [SerializeField] private float _moveRange;
    [Header("몬스터 이동 시 최소 이동거리")] // 수정할 것.
    [SerializeField] private float _minMoveDistance;
    [Header("몬스터 최대 이동 거리(이 값 넘으면 처음위치로)")]
    [SerializeField] private float _maxDistance;

    Rigidbody2D _rigidbody;

    //몬스터가 배치된 위치
    private Vector2 _initPosition;
    private Vector2 _moveDistance;
    private Vector3 _destination;
    private bool _IsSettedInitPosition;
    private bool _IsMoving;


    // Start is called before the first frame update
    void Start()
    {
        
        _IsSettedInitPosition = false;
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.I) && !_IsMoving)
        {
            SetDestination();
        }
    }

    private void FixedUpdate()
    {
        if (_IsMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, _destination, _moveSpeed * Time.deltaTime);

        }

        //목적지에 도착했을 때.
        if (_destination == transform.position && _IsMoving)
        {
            _IsMoving = false;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(!_IsSettedInitPosition) SetInitPosition();
    }
    private void SetDestination()
    {
        _moveDistance.x = Random.Range(-_moveRange, _moveRange + 1);

        //이동 거리 관련 코드 수정할 것.
        //이동 거리의 절대값이 2 이하일때 2만큼 더 움직이도록 함.
        if (_moveDistance.x <= Mathf.Abs(_minMoveDistance) && _moveDistance.x >= 0) _moveDistance.x += 2;
        if (_moveDistance.x >= -Mathf.Abs(_minMoveDistance) && _moveDistance.x <= 0) _moveDistance.x -= 2;
        // 수정할 것.


        _IsMoving = true;

        _destination = new Vector3(_moveDistance.x, transform.position.y, 0);


        //초기 위치에서 너무 멀어졌을 때
        if(Mathf.Abs(_initPosition.x - transform.position.x) > _maxDistance) ReturnToInitPosition();
    }

    private void ReturnToInitPosition()
    {
        _IsMoving = true;
        _destination = _initPosition;
    }

    private void SetInitPosition()
    {
        _initPosition = transform.position;
        _IsSettedInitPosition = true;
    }
}
