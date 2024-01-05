using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("���� �̵� �ӵ�")]
    [SerializeField] private float _moveSpeed;
    [Header("���� �̵� �Ÿ� ����(���� �� ������)")]
    [SerializeField] private float _moveRange;
    [Header("���� �̵� �� �ּ� �̵��Ÿ�")] // ������ ��.
    [SerializeField] private float _minMoveDistance;
    [Header("���� �ִ� �̵� �Ÿ�(�� �� ������ ó����ġ��)")]
    [SerializeField] private float _maxDistance;

    Rigidbody2D _rigidbody;

    //���Ͱ� ��ġ�� ��ġ
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

        //�������� �������� ��.
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

        //�̵� �Ÿ� ���� �ڵ� ������ ��.
        //�̵� �Ÿ��� ���밪�� 2 �����϶� 2��ŭ �� �����̵��� ��.
        if (_moveDistance.x <= Mathf.Abs(_minMoveDistance) && _moveDistance.x >= 0) _moveDistance.x += 2;
        if (_moveDistance.x >= -Mathf.Abs(_minMoveDistance) && _moveDistance.x <= 0) _moveDistance.x -= 2;
        // ������ ��.


        _IsMoving = true;

        _destination = new Vector3(_moveDistance.x, transform.position.y, 0);


        //�ʱ� ��ġ���� �ʹ� �־����� ��
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
