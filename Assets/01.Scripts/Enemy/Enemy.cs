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
    [SerializeField]
    private Vector3 _groundBox;
    [Header("���� ��ȸ �ӵ�")]
    [SerializeField] private float _idleMoveSpeed;
    [Header("���� ���� �ӵ�")]
    [SerializeField] private float _chaseMoveSpeed;
    [Header("���� ������")]
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _forwardForce;
    [Header("���� ���� ����")]
    [SerializeField] private float _searchingRange;
    public bool _isChasing;
    public bool _isInSight;
    [Header("���� ���� ī��Ʈ(��)")]
    [SerializeField] private float _searchingStopCount;



    [Header("���� ��ȸ �� �̵� �Ÿ�(���� �� ����)")]
    [SerializeField] private float _roamRange;
    [Header("���� ��ȸ �� �ּ� �̵��Ÿ�")]
    [SerializeField] private float _minRoamDistance;
    [Header("���� ��ȸ �ִ� �̵� �Ÿ�(�� �� ������ ó����ġ��)")]
    [SerializeField] private float _maxRoamDistance;

    [SerializeField] private Transform _jumpSensor;
    [SerializeField] private Transform _groundSensor;

    [Header("���� ���� �� ��ȸ Ƚ��")]
    [SerializeField] private float _chaseFailRoamCount;
    [Header("���� ���� ��ȸ �� ���� ��ȸ���� �ɸ��� �ð�(��)")]
    [SerializeField] private float _chaseFailRoamCoolTime;

    [Header("���� ������� �Ҹ� �ֱ�(��)")]
    [SerializeField] private float _cryingCycleTime;

    [Header("����")]
    [SerializeField] private List<AudioClip> _crySounds = new();
    [SerializeField] private AudioClip JumpScare;
    [SerializeField] private AudioClip _screamingSound;

    [SerializeField] private AudioClip _headbuttSound;

    [Header("����� ����")]
    [SerializeField][Range(0f, 1f)] private float CryVolume;
    [Header("��� ����")]
    [SerializeField][Range(0f, 1f)] private float ScreamVolume;


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
    private bool _isStartChasing;
    private float SavedDir;
    private bool _isLeftDir;
    private bool _isOnGround;

    private Coroutine _roamco;
    private Coroutine _encountco;
    private Coroutine _threatco;
    private Coroutine _idleToRoamCo;

    Rigidbody2D _rigid2d;
    Animator _anim;

    private Player _player;
    private FSM _fsm = new();
    private BaseState _idleState, _roamState, _chaseState,_chaseFailState,_returnState ,_threteningHide;

    // Start is called before the first frame update
    void Start()
    {
        _isOnGround = true;
        _isLeftDir = false;

        _player = Player.Instance;
        _fsm.CurrentState = _idleState;

        _idleState = new(   
        onEnter: () =>
        {
            Debug.Log("idle ����");

            _anim.SetBool("IsWalking", false);
            _anim.SetBool("IsRunning", false);
            _idleToRoamCo = StartCoroutine(IdleToRoam());
        },
        onUpdate: () =>
        {
            if(_isInSight && !_player.IsHidden && _fsm.CurrentState != _chaseState && _idleToRoamCo != null)
            {
                _fsm.CurrentState = _chaseState;
                StopCoroutine(_idleToRoamCo);
            }
        },
        onExit: () =>{
        });
        
        _returnState = new(
        onEnter: () =>{},
        onUpdate: () =>
        {
            MoveToDestination(InitPosition, _idleMoveSpeed);

            if(Mathf.Abs(InitPosition.x - transform.position.x) < 0.1f)
            {
                _fsm.CurrentState = _idleState;
            }

            if (_isInSight && !_player.IsHidden && _fsm.CurrentState != _chaseState)
            {
                _fsm.CurrentState = _chaseState;
            }


        },
        onExit: () =>{});
        
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
            if(Mathf.Abs(_destination.x - transform.position.x) < 0.1f)
            {
                _fsm.CurrentState = _idleState;
            }


            if (_isInSight && !_player.IsHidden && _fsm.CurrentState != _chaseState)
            {
                _fsm.CurrentState = _chaseState;
            }

            if(_isOnGround && _fsm.CurrentState != _roamState)
            {
                
            }
        },
        onExit: () => {
            
        });

        _chaseState = new(
        onEnter: () =>
        {
            _isStartChasing = true;

            _encountco = StartCoroutine(EncountWithPlayer());
            Debug.Log("�������� ����");
        },
        onUpdate: () =>
        {
            if (_player.IsHidden)
            {
                _fsm.CurrentState = _threteningHide;
            }
            else
            {
                MoveToDestination(_player.transform.position, _chaseMoveSpeed);
            }
        },
        onExit: () =>
        {
            _isStartChasing = false;
        });

        /*
        _chaseFailState = new(                                
            onEnter: () => {
                _chaseFailPosition = transform.position;
            },
            onUpdate: () => {
            if (!_player.IsHidden && _isInSight && _fsm.CurrentState != _chaseState)     //�þ� ������ ���� �ʾ��� ��.
                {
                    _fsm.CurrentState = _chaseState;
                }
            },
            onExit: () => {
            });
        */
        _threteningHide = new(onEnter: () => {
            _threatco = StartCoroutine(Threatening());
        },
            onUpdate: () => {
                if(!_player.IsHidden && _isInSight &&  _fsm.CurrentState != _chaseState)
                {
                    if(_threatco != null)
                    {
                        StopCoroutine(_threatco);
                        _anim.SetBool("IsScreaming", false);
                        _anim.SetBool("IsHeadButt", false);

                        _fsm.CurrentState = _chaseState;
                    }
                }
            },
            onExit: () => {
            });


        _isStartChasing = false;

        
        _anim = GetComponent<Animator>();
        _rigid2d = GetComponent<Rigidbody2D>();

        _chasable = true;
        _IsSettedInitPosition = false;
        _isChasing = false;
        StartCoroutine(SetDirection());

    }

    // Update is called once per frame
    void Update()
    {
        
        HandleState();

        RaycastHit2D hit = Physics2D.BoxCast(_jumpSensor.position, boxSize, 0f, Vector2.zero, 0f, LayerMask.GetMask("Pushable", "Wall"));
        if (hit.collider != null  && _isOnGround)
        {
            Jump();
        }
        RaycastHit2D GroundHit = Physics2D.BoxCast(_groundSensor.position , _groundBox,0f, Vector2.zero, 0f, LayerMask.GetMask("Pushable", "Wall"));
        if (GroundHit.collider != null )
        {
            _isOnGround = true;
            _anim.SetBool("IsJumping", false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_IsSettedInitPosition) StartCoroutine(SetInitPosition());
        //if (collision.gameObject == wall)
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
        for(int i= 0; i < _chaseFailRoamCount; i++)
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
            Debug.Log(i);
            yield return new WaitUntil(() => transform.position == _destination);
            yield return new WaitForSeconds(_chaseFailRoamCoolTime);
        }
        yield return new WaitForSeconds(1);
        TeleportToInitposition();

    }
    private IEnumerator PlayCryingSound()
    {
        int Selecter = Random.Range(0, _crySounds.Count);
        SoundManager.Instance.PlaySFX(_crySounds[Selecter], transform.position, CryVolume, 1);

        //_audioSource.volume = 0.2f;
        yield return new WaitForSeconds(_cryingCycleTime);
    }
    public IEnumerator EncountWithPlayer()
    {
        _anim.SetBool("IsScreaming", true);

        SoundManager.Instance.PlaySFX(JumpScare, transform.position, CryVolume, 1);
        SoundManager.Instance.PlaySFX(_screamingSound, Camera.main.transform.position, ScreamVolume, 1, Camera.main.transform);
        _chasable = false;

        _chaseMoveSpeed = 0;
        yield return new WaitForSeconds(1.7f);
        _chaseMoveSpeed = 5;

        _anim.SetBool("IsScreaming", false);
        _anim.SetBool("IsRunning", true);

        // _chaseMoveSpeed = SavedMoveSpeed;
        yield return new WaitForSeconds(3);
        _chasable = true;
    }
    private IEnumerator SetInitPosition()
    {
        yield return new WaitForSeconds(1);
        InitPosition = transform.position;
        _IsSettedInitPosition = true;
        _fsm.CurrentState = _idleState;
    }
    private IEnumerator SetDirection()
    {
        while (true)
        {
            if(_isOnGround)
            {
                _currentLocation = transform.position;

                yield return new WaitForSeconds(0.05f);
                _prevLocation = _currentLocation;
                _currentLocation = transform.position;
                _posGap = (_currentLocation - _prevLocation);

                if (_posGap.x < 0)
                {
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                    _isLeftDir = true;
                }
                if (_posGap.x > 0)
                {
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                    _isLeftDir = false;
                }
                if (_posGap.x == 0) { transform.rotation = transform.rotation; }
            }
            yield return new WaitForSeconds(0.05f);
        }
    }
    private IEnumerator IdleToRoam()    //idle���¿��� roam ���·� �̵�.
    {
        yield return new WaitForSeconds(2);
        _fsm.CurrentState = _roamState;
    }
    private IEnumerator roamToIoam()    
    {
        yield return new WaitForSeconds(2);
        _fsm.CurrentState = _idleState;
    }

    private IEnumerator Threatening()
    {
        while(Mathf.Abs(transform.position.x - _player.transform.position.x) >= 1.2f)
        {
            MoveToDestination(_player.transform.position, 2);
            yield return null;
        }

        _anim.SetBool("IsWalking",false);
        yield return new WaitForSeconds(2);

        _anim.SetBool("IsWalking", false);
        for (int i = 0; i < 3; i++)
        {
            _anim.SetBool("IsHeadButt", true);
            SoundManager.Instance.PlaySFX(_headbuttSound, transform.position, 1, 1);
            yield return new WaitForSeconds(1f);
            _anim.SetBool("IsHeadButt", false);
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(2);
        _anim.SetBool("IsScreaming", true);
        SoundManager.Instance.PlaySFX(_screamingSound, transform.position, CryVolume, 1);
        yield return new WaitForSeconds(1);
        _anim.SetBool("IsScreaming", false);
    }
    private void MoveToDestination(Vector3 Destination , float MoveSpeed)
    {
        if(MoveSpeed <3)
        {
            _anim.SetBool("IsWalking", true);
            _anim.SetBool("IsRunning", false);
        }
        if(MoveSpeed >= 3)
        {
            _anim.SetBool("IsRunning", true);
            _anim.SetBool("IsWalking", false);
        }
        if(MoveSpeed <=0)
        {
            _anim.SetBool("IsRunning", false);
            _anim.SetBool("IsWalking", false);
        }


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

            _anim.SetBool("IsWalking", false);
            _anim.SetBool("IsRunning", false);
        }

        _rigid2d.velocity = new Vector3(SavedDir * MoveSpeed, _rigid2d.velocity.y);
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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(_jumpSensor.position, boxSize);
        Gizmos.DrawWireCube(_groundSensor.position,_groundBox);

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject == _player.gameObject)
        {
            _isInSight = true;
        }
        if (collision.gameObject == _player.gameObject && !_player.IsHidden && _fsm.CurrentState != _chaseState && _chasable && !_isChasing)
        {
            _isChasing = true;
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
        if(_player != null && collision.gameObject == _player.gameObject)
        {
            _isInSight = false;
            
        }
    }

    private void Jump()
    {
        _anim.SetBool("IsJumping", true);

        _anim.SetBool("IsWalking", false);
        _anim.SetBool("IsRunning", false);

        _isOnGround = false;
        _rigid2d.velocity = Vector2.up * _jumpForce + (_forwardForce * (Vector2)transform.right) ;
    }
}
