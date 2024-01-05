using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("몬스터 이동 속도")]
    [SerializeField] private float _moveSpeed;
    [Header("몬스터 이동 거리 범위(범위 내 랜덤임)")]
    [SerializeField] private float _moveRange;
    [Header("몬스터 이동 시 최소 이동거리")] 
    [SerializeField] private float _minMoveDistance;
    [Header("몬스터 최대 이동 거리(이 값 넘으면 처음위치로)")]
    [SerializeField] private float _maxDistance;

    Rigidbody2D _rigidbody;

    //몬스터가 배치된 위치
    private Vector2 _initPosition;
    private Vector2 _moveDistance;
    private Vector3 _destination;
    private bool _IsSettedInitPosition;


    // Start is called before the first frame update
    void Start()
    {
        _IsSettedInitPosition = false;
        _rigidbody = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            SetDestination();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(!_IsSettedInitPosition) SetInitPosition();

        //if (collision.gameObject == wall)
        _destination = transform.position;
    }
    private void SetDestination()
    {
        _moveDistance.x = Random.Range(-_moveRange, _moveRange + 1);

        //이동 거리의 절대값이 minMoveDistance 이하일때 minMoveDistance만큼 움직이도록 함.
        if (_moveDistance.x <= _minMoveDistance && _moveDistance.x >= 0) _moveDistance.x = _minMoveDistance;
        if (_moveDistance.x >= -_minMoveDistance && _moveDistance.x <= 0) _moveDistance.x = -_minMoveDistance;

        _destination = new Vector3(_moveDistance.x, transform.position.y, 0);

        //초기 위치에서 너무 멀어졌을 때
        if(Mathf.Abs(_initPosition.x - transform.position.x) > _maxDistance) ReturnToInitPosition();

        StartCoroutine(Moving());
    }
    IEnumerator Moving()
    {
        while (_destination != transform.position) {
            transform.position = Vector3.MoveTowards(transform.position, _destination, _moveSpeed * Time.deltaTime);

            yield return null;
        }
    }
    private void ReturnToInitPosition()
    {
        _destination = _initPosition;
    }

    private void SetInitPosition()
    {
        _initPosition = transform.position;
        _IsSettedInitPosition = true;
    }
}
