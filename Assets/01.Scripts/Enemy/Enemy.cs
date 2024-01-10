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

    [Header("몬스터 배회 속도")]
    [SerializeField] private float _idleMoveSpeed;
    [Header("몬스터 추적 속도")]
    [SerializeField] private float _chaseMoveSpeed;
    private float _savedChaseSpeed;
    [Header("몬스터 점프력")]
    [SerializeField] private float _jumpForce;

    [Header("추적 시작 범위")]
    [SerializeField] private float _searchingRange;
    public bool _isChasing;
    public bool _isInSight;
    [Header("추적 종료 카운트(초)")]
    [SerializeField] private float _searchingStopCount;



    [Header("몬스터 배회 시 이동 거리(범위 내 랜덤)")]
    [SerializeField] private float _roamRange;
    [Header("몬스터 배회 시 최소 이동거리")]
    [SerializeField] private float _minRoamDistance;
    [Header("몬스터 배회 최대 이동 거리(이 값 넘으면 처음위치로)")]
    [SerializeField] private float _maxRoamDistance;

    [SerializeField] private Transform _jumpSensor;

    [Header("감지 범위")]
    [SerializeField] private float _detectRange;
    [Header("수색 범위")]
    [SerializeField] private float _searchRange;
    [Header("수색 시 한쪽으로 간 후 대기하는 시간(초)")]
    [SerializeField] private float _searchWaitTime;
    [Header("수색 실패 시 배회 횟수")]
    [SerializeField] private float _chaseFailRoamCount;
    [Header("수색 실패 배회 시 다음 배회까지 걸리는 시간(초)")]
    [SerializeField] private float _chaseFailRoamCoolTime;

    [Header("몬스터 흐느끼는 소리 주기(초)")]
    [SerializeField] private float _cryingCycleTime;


    [Header("사운드")]
    [SerializeField] private AudioClip Crying1;
    [SerializeField] private AudioClip Crying2;
    [SerializeField] private AudioClip Crying3;

    [SerializeField] private AudioClip JumpScare;
    [SerializeField] private AudioClip Screaming;

    [Header("흐느낌 볼륨")]
    [SerializeField][Range(0f, 1f)] private float CryVolume;
    [Header("비명 볼륨")]
    [SerializeField][Range(0f, 1f)] private float ScreamVolume;
    List<AudioClip> CrySounds = new List<AudioClip>();


    //몬스터가 배치된 위치
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

    Rigidbody2D _rigid2d;
    

    private Player _player;

    private GameObject _screamParticle;

    private FSM _fsm = new();
    private BaseState _idleState, _roamState, _chaseState,_chaseFailState,_returnState;

    // Start is called before the first frame update
    void Start()
    {
        _savedChaseSpeed = _chaseMoveSpeed;
        _player = Player.Instance;
        _fsm.CurrentState = _idleState;

        _idleState = new(   
        onEnter: () =>
        {
            Debug.Log("idle 상태");
        },
        onUpdate: () =>
        {
            if(_isInSight && !_player.IsHidden && _fsm.CurrentState != _chaseState)
            {
                _fsm.CurrentState = _chaseState;
            }
            //MoveToDestination(transform.position,_idleMoveSpeed);
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
            //----목표 설정----
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
        onExit: () =>
        {
        });

        _chaseState = new(
        onEnter: () =>
        {
            _isStartChasing = true;
            StartCoroutine(EncountWithPlayer());


            Debug.Log("추적상태 진입");
        },
        onUpdate: () =>
        {
            if (_isChasing && !_player.IsHidden)              //추적중이면서 숨지 않았을 때. : 계속 쫓아간다.
            {
                MoveToDestination(_player.transform.position, _chaseMoveSpeed);
            }
            if(!_isChasing && _player.IsHidden && _isInSight)                //추적 끝,숨었을 때. 추적 실패.
            {
                _fsm.CurrentState = _chaseFailState;
            }

            if(!_isChasing && !_isInSight )
            {
                _fsm.CurrentState = _chaseFailState;
            }  //숨지 않았지만 시야 내부에 없고 추적이 끝남. 추적 실패.


        },
        onExit: () =>
        {
            _isStartChasing = false;
        });

        //플레이어를
        //놓쳤을 때(_isplayerinsight가 false 일때).
        _chaseFailState = new(                                
            onEnter: () => {
                _chaseFailPosition = transform.position;

                Debug.Log("추적실패상태 진입");

                if (_player.IsHidden) 
                {
                    //플레이어가 숨어있을 때의 함수를 실행할 것. 머리박는 애니메이션, 조금 후 소리지르고 이후 초기 위치로 복귀.
                    //StartCoroutine(RoamNear());
                    Debug.Log("플레이어가 숨어있을 때의 함수를 실행할 것. 머리박는 애니메이션, 조금 후 소리지르고 이후 초기 위치로 복귀.");
                    TeleportToInitposition();
                }
                if(!_player.IsHidden)
                {
                    //감지범위를 벗어났을 때. 주변 배회 후 초기 위치로 복귀
                    Debug.Log("감지범위를 벗어났을 때. 주변 배회 후 초기 위치로 복귀");
                    TeleportToInitposition();
                }
            },
            onUpdate: () => {
            if (!_player.IsHidden && _isInSight && _fsm.CurrentState != _chaseState)     //시야 내에서 숨지 않았을 때. ( chase fail로 넘어왔다가 다시 활성화 되는 경우는 숨었다가 범위내에서 나왔을 때임.)
                {
                    _fsm.CurrentState = _chaseState;
                }
            },
            onExit: () => { 
                
            });


        _isStartChasing = false;
        _rigid2d = GetComponent<Rigidbody2D>();
        _chasable = true;
        _IsSettedInitPosition = false;
        _isChasing = false;
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
            //Debug.Log("BoxCast hit: " + hit.collider.name);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            Jump();
        }

        if(!_player.IsHidden && _isInSight && !_isChasing)
        {
            _isChasing = true;
            _fsm.CurrentState = _chaseState;
        } 

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_IsSettedInitPosition) StartCoroutine(SetInitPosition());
        //if (collision.gameObject == wall)
        Debug.Log("벽과 충돌함");
        _destination = transform.position;
    }
    /* //수색-추적 코루틴
    private IEnumerator Searching_2()
    {
        int Selecter = Random.Range(0, 2);

        Vector2 FirstSearchDestination = _searchDesntinationLeft;
        Vector2 SecondSearchDesntination = _searchDesntinationRight;

        _destination = new Vector3(FirstSearchDestination.x,FirstSearchDestination.y,0);
        yield return Moving(_destination);
        yield return new WaitUntil(() => new Vector2(transform.position.x, transform.position.y) == FirstSearchDestination); // 대리자, 람다식.

        
        

        yield return new WaitForSeconds(_searchWaitTime);
        _destination = new Vector3(SecondSearchDesntination.x,SecondSearchDesntination.y,0);
        yield return Moving(_destination);

        yield return new WaitUntil(() => new Vector2(transform.position.x, transform.position.y) == SecondSearchDesntination); // 대리자, 람다식.
        
        yield return new WaitForSeconds(_searchWaitTime);
        ReturnToInitPosition();
    } //수색-추적*/
    /* //배회 코루틴
    private IEnumerator Searching_1() // 배회
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
    /* //이동 코루틴
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
        TeleportToInitposition();

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
        yield return new WaitForSeconds(0.7f);
        
        _screamParticle.SetActive(true);
        yield return new WaitForSeconds(1);
       // _chaseMoveSpeed = SavedMoveSpeed;
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
    private IEnumerator SetDirection()
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
    private IEnumerator StopChasing()
    {
        yield return new WaitForSeconds(_searchingStopCount);
        _isChasing = false;
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
        // 현재 오브젝트의 위치를 기준으로 상자를 그립니다.
        Gizmos.DrawWireCube(_jumpSensor.position, boxSize);
        

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

        
        if(_isChasing && !_player.IsHidden)
        {
            StopCoroutine(StopChasing());
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
            _isInSight = false;
            
        }
        if (!_isInSight && _isChasing)
        {
            StartCoroutine(StopChasing());
        }
    }

    private void Jump()
    {
        _rigid2d.velocity = Vector2.up * _jumpForce;
    }
}
