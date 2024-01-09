using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private Vector3 boxSize;
    [Header("���� ��ȸ �ӵ�")]
    [SerializeField] private float _idleMoveSpeed;
    [Header("���� ���� �ӵ�")]
    [SerializeField] private float _chaseMoveSpeed;
    [Header("���� ������")]
    [SerializeField] private float _jumpForce;

    [Header("���� ���� ����")]
    [SerializeField] private float _searchingRange;
    private bool _isPlayerInSight;

    [Header("���� ��ȸ �� �̵� �Ÿ�(���� �� ����)")]
    [SerializeField] private float _roamRange;
    [Header("���� ��ȸ �� �ּ� �̵��Ÿ�")]
    [SerializeField] private float _minRoamDistance;
    [Header("���� ��ȸ �ִ� �̵� �Ÿ�(�� �� ������ ó����ġ��)")]
    [SerializeField] private float _maxRoamDistance;

    [SerializeField] private Transform _jumpSensor;

    [Header("���� ����")]
    [SerializeField] private float _detectRange;
    [Header("���� ����")]
    [SerializeField] private float _searchRange;
    [Header("���� �� �������� �� �� ����ϴ� �ð�(��)")]
    [SerializeField] private float _searchWaitTime;
    [Header("���� ���� �� ��ȸ Ƚ��")]
    [SerializeField] private float _chaseFailRoamCount;
    [Header("���� ���� ��ȸ �� ���� ��ȸ���� �ɸ��� �ð�(��)")]
    [SerializeField] private float _chaseFailRoamCoolTime;

    [Header("���� ������� �Ҹ� �ֱ�(��)")]
    [SerializeField] private float _cryingCycleTime;


    [Header("����")]
    [SerializeField] private AudioClip Crying1;
    [SerializeField] private AudioClip Crying2;
    [SerializeField] private AudioClip Crying3;

    [SerializeField] private AudioClip JumpScare;
    [SerializeField] private AudioClip Screaming;

    [Header("����� ����")]
    [SerializeField][Range(0f, 1f)] private float CryVolume;
    [Header("��� ����")]
    [SerializeField][Range(0f, 1f)] private float ScreamVolume;
    List<AudioClip> CrySounds = new List<AudioClip>();


    //���Ͱ� ��ġ�� ��ġ
    public Vector2 InitPosition;
    private Vector3 _destination;
    private bool _IsSettedInitPosition;
    private Vector2 _currentLocation;
    private Vector2 _prevLocation;
    private Vector2 _posGap;
    private Vector2 _chaseFailPosition;
    private int _direction;
    private bool _chasable;
    private float SavedDir;

    Rigidbody2D _rigid2d;
    

    private Player _player;

    private GameObject _screamParticle;

    private FSM _fsm = new();
    private BaseState _idleState, _roamState, _chaseState,_chaseFailState,_returnState;

    // Start is called before the first frame update
    void Start()
    {
        
        _player = Player.Instance;
        _fsm.CurrentState = _idleState;

        _idleState = new(
        onEnter: () =>
        {
            
        },
        onUpdate: () =>
        {
            MoveToDestination(transform.position,_idleMoveSpeed);
        },
        onExit: () =>
        {

        });
        
        _returnState = new(
        onEnter: () =>
        {
            

        },
        onUpdate: () =>
        {
            MoveToDestination(InitPosition, _idleMoveSpeed);
        },
        onExit: () =>
        {
            
        });
        
        _roamState = new(
        onEnter: () =>
        {
            //----��ǥ ����----
            float Distance = Random.Range(-_roamRange, _roamRange + 0.1f);
            if (Mathf.Abs(Distance) <= _minRoamDistance && Distance >= 0) Distance = _minRoamDistance;
            if (Mathf.Abs(Distance) <= _minRoamDistance && Distance < 0) Distance = -_minRoamDistance;

            if (Mathf.Abs(transform.position.x - InitPosition.x) <= _maxRoamDistance) _destination = new Vector3(transform.position.x + Distance, transform.position.y, 0);
            else _fsm.CurrentState = _returnState;
        },
        onUpdate: () =>
        {
            MoveToDestination(_destination, _idleMoveSpeed);
        },
        onExit: () =>
        {
        });

        _chaseState = new(
        onEnter: () =>
        {
            StartCoroutine(EncountWithPlayer());
        },
        onUpdate: () =>
        {
            if (_isPlayerInSight && !_player.IsHidden)              //�þ߹��� �ȿ� �����鼭 ���� �ʾ��� ��. : ��� �Ѿư���.
            {
                MoveToDestination(_player.transform.position, _chaseMoveSpeed);
            }
            if(_isPlayerInSight && _player.IsHidden)                //�þ߹��� �ȿ� �����鼭 ������ ��. ���� ����.
            {
                _fsm.CurrentState = _chaseFailState;
            }
            if (!_isPlayerInSight)                                   //�þ߹������� ������ ��. ���� ����.
            {
                _fsm.CurrentState = _chaseFailState;
            }

        },
        onExit: () =>
        {

        });

        //�÷��̾ �þ߹������� ������ ��.
        _chaseFailState = new(                                
            onEnter: () => {
                _chaseFailPosition = transform.position;

                if(_player.IsHidden)
                {
                    //�÷��̾ �������� ���� �Լ��� ������ ��. �Ӹ��ڴ� �ִϸ��̼�, ���� �� �Ҹ������� ���� �ʱ� ��ġ�� ����.
                    Debug.Log("�÷��̾ ������.");

                }

                if(!_player.IsHidden)
                {
                    //���������� ����� ��. �ֺ� ��ȸ �� �ʱ� ��ġ�� ����
                    Debug.Log("�������� ���!");
                    StartCoroutine(RoamNear());

                }
            },
            onUpdate: () => {

            },
            onExit: () => { 
                
            });

        _rigid2d = GetComponent<Rigidbody2D>();
        _chasable = true;
        _IsSettedInitPosition = false;
        _isPlayerInSight = false;
        StartCoroutine(SetDirection());

        _screamParticle = transform.GetChild(0).gameObject;
        _screamParticle.SetActive(false);

        CrySounds.Add(Crying1);
        CrySounds.Add(Crying2);
        CrySounds.Add(Crying3);
    }

    // Update is called once per frame
    void Update()
    {
        HandleState();
        RaycastHit2D hit = Physics2D.BoxCast(_jumpSensor.position, boxSize, 0f, Vector2.zero, 0f);
        if (hit.collider != null && hit.collider.gameObject != this.gameObject)
        {
            Debug.Log("BoxCast hit: " + hit.collider.name);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            Jump();
        }

        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_IsSettedInitPosition) StartCoroutine(SetInitPosition());


        //if (collision.gameObject == wall)
        Debug.Log("���� �浹��");
        _destination = transform.position;
    }
    /* //����-���� �ڷ�ƾ
    private IEnumerator Searching_2()
    {
        int Selecter = Random.Range(0, 2);

        Vector2 FirstSearchDestination = _searchDesntinationLeft;
        Vector2 SecondSearchDesntination = _searchDesntinationRight;

        _destination = new Vector3(FirstSearchDestination.x,FirstSearchDestination.y,0);
        yield return Moving(_destination);
        yield return new WaitUntil(() => new Vector2(transform.position.x, transform.position.y) == FirstSearchDestination); // �븮��, ���ٽ�.

        
        

        yield return new WaitForSeconds(_searchWaitTime);
        _destination = new Vector3(SecondSearchDesntination.x,SecondSearchDesntination.y,0);
        yield return Moving(_destination);

        yield return new WaitUntil(() => new Vector2(transform.position.x, transform.position.y) == SecondSearchDesntination); // �븮��, ���ٽ�.
        
        yield return new WaitForSeconds(_searchWaitTime);
        ReturnToInitPosition();
    } //����-����*/
    /* //��ȸ �ڷ�ƾ
    private IEnumerator Searching_1() // ��ȸ
    {
        
        while(_currentState == State.Walk)
        {

            float Distance = Random.Range(-_roamRange, _roamRange + 0.1f);
            if (Mathf.Abs(Distance) <= _minRoamDistance && Distance >= 0)
            {
                Distance = _minRoamDistance;
            }
            if (Mathf.Abs(Distance) <= _minRoamDistance && Distance < 0)
            {
                Distance = -_minRoamDistance;
            }

            if (Mathf.Abs(transform.position.x - InitPosition.x) <= _maxRoamDistance)
            {
                _destination = new Vector3(transform.position.x + Distance, transform.position.y, 0);
                StartCoroutine(Moving(_destination));
            }
            if (Mathf.Abs(transform.position.x - InitPosition.x) >= _maxRoamDistance) { ReturnToInitPosition(); }

            yield return new WaitUntil(() => _destination == transform.position);


        }
    }
    */
    /* //�̵� �ڷ�ƾ
    private IEnumerator Moving(Vector3 Destination)
    {
        while (Destination != transform.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, Destination, _moveSpeed * Time.deltaTime);

            yield return null;
        }
    }
    */
    private IEnumerator RoamNear()
    {
        for(int i = 0; i < _chaseFailRoamCount; i++)
        {
            float Distance = Random.Range(-_roamRange, _roamRange + 0.1f);
            if (Mathf.Abs(Distance) <= _minRoamDistance && Distance >= 0) Distance = _minRoamDistance;
            if (Mathf.Abs(Distance) <= _minRoamDistance && Distance < 0) Distance = -_minRoamDistance;

            if (Mathf.Abs(transform.position.x - _chaseFailPosition.x) <= _maxRoamDistance) _destination = new Vector3(transform.position.x + Distance, transform.position.y, 0);

            while (transform.position != _destination)
            {
                MoveToDestination(_destination, _idleMoveSpeed);
                yield return null;
            }

            yield return new WaitUntil(() => transform.position == _destination);
            yield return new WaitForSeconds(_chaseFailRoamCoolTime);
        }
        yield return new WaitForSeconds(1);
        Debug.Log("�������� ��ȸ ����.");
        TeleportToInitposition();
        _fsm.CurrentState = _idleState;

    }
    private IEnumerator PlayCryingSound()
    {
        int Selecter = Random.Range(0, 3);
        SoundManager.Instance.PlaySFX(CrySounds[Selecter], transform.position, CryVolume, 1);

        //_audioSource.volume = 0.2f;
        yield return new WaitForSeconds(_cryingCycleTime);
    }
    public IEnumerator EncountWithPlayer()
    {
        SoundManager.Instance.PlaySFX(JumpScare, transform.position, CryVolume, 1);
        SoundManager.Instance.PlaySFX(Screaming, Camera.main.transform.position, ScreamVolume, 1, Camera.main.transform);
        _chasable = false;
        float SavedMoveSpeed = _chaseMoveSpeed;
        _chaseMoveSpeed = 0;
        yield return new WaitForSeconds(0.7f);
        
        _screamParticle.SetActive(true);
        yield return new WaitForSeconds(1);
        _chaseMoveSpeed = SavedMoveSpeed;
        yield return new WaitForSeconds(3);
        _chasable = true;
        _screamParticle.SetActive(false);
    }
    private IEnumerator SetInitPosition()
    {
        yield return new WaitForSeconds(1);
        InitPosition = transform.position;
        _IsSettedInitPosition = true;
    }
    private IEnumerator SetDirection() //0108. ������⿡ ���� �ٶ󺸴� ������ �޶�������.
    {
        while(true)
        {
            _currentLocation = transform.position;

            yield return new WaitForSeconds(0.05f);
            _prevLocation = _currentLocation;
            _currentLocation = transform.position;
            _posGap = (_currentLocation - _prevLocation);

            if(_posGap.x <0) { transform.rotation = Quaternion.Euler(0, 180,0); }
            if (_posGap.x > 0) { transform.rotation = Quaternion.Euler(0, 0, 0); }
            if (_posGap.x == 0) { transform.rotation = transform.rotation; }
        }
    }
    private void MoveToDestination(Vector3 Destination , float MoveSpeed)
    {
        
        if(Mathf.Abs(Destination.x - transform.position.x) > 0.1f)
        {
            float dirx;
            dirx = Destination.x - transform.position.x;
            if (dirx < 0) { dirx = -1; SavedDir = dirx; }
            if (dirx > 0) { dirx = 1; SavedDir = dirx; }
        }
        if(Mathf.Abs(Destination.x - transform.position.x) <= 0.1f)
        {
            SavedDir = 0;
            _destination = transform.position;
        }

        transform.position += new Vector3(SavedDir * MoveSpeed * Time.deltaTime, 0, 0);
    }
    private void HandleState()
    {
        _fsm.Update();

        if (Input.GetKeyDown("1"))
        {
            Debug.Log("idle");
            _fsm.CurrentState = _idleState;
        }
        if (Input.GetKeyDown("2"))
        {
            Debug.Log("roam");
            _fsm.CurrentState = _roamState;
        }
        if (Input.GetKeyDown("3"))
        {
            Debug.Log("chase");
            _fsm.CurrentState = _chaseState;
        }
        if (Input.GetKeyDown("4"))
        {
            Debug.Log("return");
            _fsm.CurrentState = _returnState;
        }
        if (Input.GetKeyDown("5"))
        {
            Debug.Log("chasefail");
            _fsm.CurrentState = _chaseFailState;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        // ���� ������Ʈ�� ��ġ�� �������� ���ڸ� �׸��ϴ�.
        Gizmos.DrawWireCube(_jumpSensor.position, boxSize);
        

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject == _player.gameObject && !_player.IsHidden && _fsm.CurrentState != _chaseState && _chasable)
        {
            _isPlayerInSight = true;
            Debug.Log("CHASE ���·� ��ȯ.");
            _fsm.CurrentState = _chaseState;
        }
    }

    private void TeleportToInitposition()
    {
        transform.position = InitPosition;
        _fsm.CurrentState = _idleState;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject == _player.gameObject)
        {
            _isPlayerInSight = false;
        }
    }

    private void Jump()
    {
        _rigid2d.velocity = Vector2.up * _jumpForce;
    }
}
