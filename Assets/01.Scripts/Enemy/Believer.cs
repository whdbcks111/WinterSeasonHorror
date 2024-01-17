using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Believer : MonoBehaviour
{
    [SerializeField]
    private Vector3 boxSize;
    [SerializeField]
    private Vector3 _groundBox;
    [SerializeField]
    private Vector2 _wallSensorSize;

    [SerializeField] private float _idleMoveSpeed;
    [SerializeField] private float _chaseMoveSpeed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _forwardForce;


    [SerializeField] private float _threatMaxDistance;
    public bool _isChasing;
    public bool _isInSight;
    [SerializeField] private float _searchingStopCount;
    [SerializeField] private float _roamRange;
    [SerializeField] private float _minRoamDistance;
    [SerializeField] private float _maxRoamDistance;

    [SerializeField] private Transform _jumpSensor;
    [SerializeField] private Transform _groundSensor;
    [SerializeField] private Transform _wallSensor;

    [SerializeField] private float _chaseFailRoamCount;
    [SerializeField] private float _chaseFailRoamCoolTime;
    [SerializeField] private float _cryingCycleTime;
    [SerializeField] private List<AudioClip> _crySounds = new();
    [SerializeField] private AudioClip JumpScare;
    [SerializeField] private AudioClip _screamingSound;
    [SerializeField] private AudioClip _headbuttSound;
    [SerializeField][Range(0f, 1f)] private float CryVolume;
    [SerializeField][Range(0f, 1f)] private float ScreamVolume;
    public Vector2 InitPosition;
    private Vector3 _destination;
    private bool _IsSettedInitPosition;
    private Vector2 _currentLocation;
    private Vector2 _prevLocation;
    private Vector2 _posGap;
    private Vector2 _chaseFailPosition;
    private int _direction;
    private bool _chasable;

    private float _savedDir;

    private bool _isOnGround;
    private bool _jumpable;
    private bool _isThereWall;

    private Coroutine _roamco;
    private Coroutine _encountco;
    private Coroutine _threatco;
    private Coroutine _idleToRoamCo;

    private Camera _camera;
    private Vector2 _objScreenPoint;

    Rigidbody2D _rigid2d;
    Animator _anim;

    private Player _player;

    public FSM _fsm = new();
    public BaseState _idleState, _roamState, _chaseState,_chaseFailState,_returnState ,_threteningHide,_getOrdered;

    // Start is called before the first frame update
    void Start()
    {
        _jumpable = true;
        _isOnGround = true;

        _player = Player.Instance;
        _fsm.CurrentState = _idleState;

        _idleState = new(   
        onEnter: () =>
        {

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
            if(_idleToRoamCo != null) StopCoroutine(_idleToRoamCo);
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


            if (_objScreenPoint.x < 0 || _objScreenPoint.x > Screen.width)
            {
                // 오브젝트가 화면 밖으로 나갔을 때
                TeleportToInitposition();
            }

            RaycastHit2D WallHit = Physics2D.BoxCast(_wallSensor.position, _wallSensorSize, 0f, Vector2.zero, 0f, LayerMask.GetMask("Pushable", "Wall"));
            if (WallHit.collider != null)
            {
                //앞에 벽이 있음. 
                if(InitPosition.x >0)
                {
                    MoveToDestination(new Vector2(-Mathf.Abs(InitPosition.x + 1000),transform.position.y),_idleMoveSpeed); //반대 방향으로 갈 것.
                }
                else MoveToDestination(new Vector2(Mathf.Abs(InitPosition.x + 1000), transform.position.y), _idleMoveSpeed);
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
        },
        onExit: () => {
            
        });

        _chaseState = new(
        onEnter: () =>
        {

            _encountco = StartCoroutine(EncountWithPlayer());
        },
        onUpdate: () =>
        {
            if (_player.IsHidden)
            {
                if(Mathf.Abs(_player.transform.position.x -transform.position.x) <= _threatMaxDistance)
                {
                    _fsm.CurrentState = _threteningHide;
                }
                if(Mathf.Abs(_player.transform.position.x - transform.position.x) > _threatMaxDistance)
                {
                    _fsm.CurrentState = _chaseFailState;
                }

                if (_objScreenPoint.x < 0 || _objScreenPoint.x > Screen.width)
                {
                    // 오브젝트가 화면 밖으로 나갔을 때
                    TeleportToInitposition();
                }
            }
            else
            {
                MoveToDestination(_player.transform.position, _chaseMoveSpeed);

                if((_player.transform.position.x - transform.position.x) > 0)
                {
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                else transform.rotation = Quaternion.Euler(0, 180, 0);
            }
        },
        onExit: () =>
        {
            if (_encountco != null) StopCoroutine(_encountco);
        });

        _chaseFailState = new(                                
            onEnter: () => {
                _chaseFailPosition = transform.position;
                _roamco = StartCoroutine(RoamNear());
            },
            onUpdate: () => {
            if (!_player.IsHidden && _isInSight && _fsm.CurrentState != _chaseState)   
                {
                    _fsm.CurrentState = _chaseState;
                    if(_roamco != null)
                    {
                        StopCoroutine(_roamco);
                    }
                }
            
            },
            onExit: () => {
                if(_roamco != null) StopCoroutine(_roamco);
            });
        
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

                if ((_player.transform.position.x - transform.position.x) > 0)
                {
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                else transform.rotation = Quaternion.Euler(0, 180, 0);
            },
            onExit: () => {
                _jumpable = true;
                _rigid2d.constraints = RigidbodyConstraints2D.None;
                _rigid2d.constraints = RigidbodyConstraints2D.FreezeRotation;
            });


        _camera = Camera.main;
        _anim = GetComponent<Animator>();
        _rigid2d = GetComponent<Rigidbody2D>();

        _chasable = true;
        _IsSettedInitPosition = false;
        _isChasing = false;
        StartCoroutine(SetDirection());
        StartCoroutine(StuckCheck());
    }

    // Update is called once per frame
    void Update()
    {
        _objScreenPoint = _camera.WorldToScreenPoint(transform.position);
        HandleState();

        
        if(Player.Instance.IsHidden)
        {
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Player"));
        }
        else Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Player"),false);


        RaycastHit2D hit = Physics2D.BoxCast(_jumpSensor.position, boxSize, 0f, Vector2.zero, 0f, LayerMask.GetMask("Pushable", "Wall"));
        if (hit.collider != null  && _isOnGround && _jumpable)
        {
            Jump();
        }
        RaycastHit2D GroundHit = Physics2D.BoxCast(_groundSensor.position , _groundBox,0f, Vector2.zero, 0f, LayerMask.GetMask("Pushable", "Wall"));
        if (GroundHit.collider != null )
        {
            _isOnGround = true;
            _anim.SetBool("IsJumping", false);
        }

        RaycastHit2D WallHit = Physics2D.BoxCast(_wallSensor.position, _wallSensorSize, 0f, Vector2.zero, 0f, LayerMask.GetMask("Pushable", "Wall"));
        if (WallHit.collider != null)
        {
            //앞에 벽이 있음. 
            _jumpable = false;
        }
        if (WallHit.collider == null)
        {
            _jumpable = true;//벽이 없음
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
            yield return new WaitUntil(() => transform.position == _destination);
            yield return new WaitForSeconds(_chaseFailRoamCoolTime);
        }
        yield return new WaitForSeconds(1);

        if (_objScreenPoint.x < 0 || _objScreenPoint.x > Screen.width)
        {
            // 오브젝트가 화면 밖으로 나갔을 때
            TeleportToInitposition();
        }
        else //화면 안에 있을 때.
        {
            _fsm.CurrentState = _returnState;
        }

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
            if(_isOnGround && _fsm.CurrentState != _chaseState &&  _fsm.CurrentState != _threteningHide)
            {
                _currentLocation = transform.position;

                yield return new WaitForSeconds(0.05f);
                _prevLocation = _currentLocation;
                _currentLocation = transform.position;
                _posGap = (_currentLocation - _prevLocation);

                if (_posGap.x < 0)
                {
                    transform.eulerAngles = new(0, 180, 0);
                }
                if (_posGap.x > 0)
                {
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                if (_posGap.x == 0) { transform.rotation = transform.rotation; }
            }
            yield return new WaitForSeconds(0.05f);
        }
    }
    private IEnumerator IdleToRoam() 
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
        _rigid2d.constraints = RigidbodyConstraints2D.FreezePositionX;
        yield return new WaitForSeconds(2);

        _jumpable = false;
        _anim.SetBool("IsWalking", false);
        _rigid2d.constraints = RigidbodyConstraints2D.None;
        _rigid2d.constraints = RigidbodyConstraints2D.FreezeRotation;
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

        _jumpable = true;
        _fsm.CurrentState = _chaseFailState;
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
            if (dirx < 0) { dirx = -1; _savedDir = dirx; }
            if (dirx > 0) { dirx = 1; _savedDir = dirx; }
        }
        if(Mathf.Abs(Destination.x - transform.position.x) <= 0.1f)
        {
            _savedDir = 0;
            _destination = transform.position;

            _anim.SetBool("IsWalking", false);
            _anim.SetBool("IsRunning", false);
        }

        _rigid2d.velocity = new Vector3(_savedDir * MoveSpeed, _rigid2d.velocity.y);
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
    
    private IEnumerator StuckCheck()
    {
        while (true)
        {
            if(_fsm.CurrentState == _roamState || _fsm.CurrentState == _chaseFailState || _fsm.CurrentState == _idleState)
            {
                float PrevLocation = transform.position.x;
                yield return new WaitForSeconds(1.5f);
                float CurrentLocation = transform.position.x;

                if (Mathf.Abs(PrevLocation - CurrentLocation) <= 0.5f) _destination = transform.position;
            }
            yield return null;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(_jumpSensor.position, boxSize);
        Gizmos.DrawWireCube(_groundSensor.position,_groundBox);
        Gizmos.DrawWireCube(_wallSensor.position, _wallSensorSize);
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
