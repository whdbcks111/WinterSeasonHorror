using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    [Header("몬스터 배회 속도")]
    [SerializeField] private float _idleMoveSpeed;
    [Header("몬스터 추적 속도")]
    [SerializeField] private float _chaseMoveSpeed;
    [Header("추적 시작 범위")]
    [SerializeField] private float _searchingRange;
    private bool _isPlayerInSight;

    [Header("몬스터 배회 시 이동 거리(범위 내 랜덤)")]
    [SerializeField] private float _roamRange;
    [Header("몬스터 배회 시 최소 이동거리")]
    [SerializeField] private float _minRoamDistance;
    [Header("몬스터 배회 최대 이동 거리(이 값 넘으면 처음위치로)")]
    [SerializeField] private float _maxRoamDistance;



    [Header("감지 범위")]
    [SerializeField] private float _detectRange;
    [Header("수색 범위")]
    [SerializeField] private float _searchRange;
    [Header("수색 시 한쪽으로 간 후 대기하는 시간(초)")]
    [SerializeField] private float _searchWaitTime;

    private Vector2 _searchDesntinationLeft;
    private Vector2 _searchDesntinationRight;

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
    private float _currentLocationX;
    private float _prevLocationX;
    private float _direction;


    private Player _player;

    private GameObject _screamParticle;

    private FSM _fsm = new();
    private BaseState _idleState, _roamState, _chaseState,_returnState;

    // Start is called before the first frame update
    void Start()
    {
        
        _player = Player.Instance;
        _fsm.CurrentState = _idleState;

        _idleState = new(
        onEnter: () =>
        {
            _destination = transform.position;
        },
        onUpdate: () =>
        {
            MoveToDestination(_destination);
        },
        onExit: () =>
        {

        });
        
        _returnState = new(
        onEnter: () =>
        {
            _destination = InitPosition;

        },
        onUpdate: () =>
        {
            MoveToDestination(_destination);
        },
        onExit: () =>
        {
            
        });
        
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

            MoveToDestination(_destination);
        },
        onExit: () =>
        {
        });

        _chaseState = new(
        onEnter: () =>
        {
            StartCoroutine(EncountWithPlayer());
            Debug.Log("조우");
        },
        onUpdate: () =>
        {
            ChasingPlayer();
        },
        onExit: () =>
        {

        });


        _direction = 1;

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
        if (Input.GetKeyDown(KeyCode.U))
        {
            StartCoroutine(PlayCryingSound());
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            StartCoroutine(EncountWithPlayer());
        }

        


        SearchingPlayer();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_IsSettedInitPosition) StartCoroutine(SetInitPosition());

        //if (collision.gameObject == wall)
        //_destination = transform.position;
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
        yield return new WaitForSeconds(0.7f);
        _screamParticle.SetActive(true);
        yield return new WaitForSeconds(4);
        _screamParticle.SetActive(false);
    }
    private IEnumerator SetInitPosition()
    {
        yield return new WaitForSeconds(1);
        InitPosition = transform.position;

        _searchDesntinationLeft = new Vector2(InitPosition.x - _searchRange, transform.position.y);
        _searchDesntinationRight = new Vector2(InitPosition.x + _searchRange, transform.position.y);

        _IsSettedInitPosition = true;
    }
    
    //0108 계속----------------------------------
    private IEnumerator SetDirection()
    {
        while(true)
        {
            _currentLocationX = transform.position.x;
            yield return null; //다음 프레임까지 대기.
            _prevLocationX = _currentLocationX;

            _direction = (_currentLocationX - _prevLocationX);

            if(_direction < 0 ) { _direction = -1; }
            if(_direction > 0) { _direction = 1; }
            if (_direction == 0) { _direction = _direction; }
        }
    }
    private void MoveToDestination(Vector3 Destination)
    {
        transform.position = Vector3.MoveTowards(transform.position,Destination, _idleMoveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, _destination) < 0.1f)
        {
            _fsm.CurrentState = _idleState;
        }
    }

    private void SearchingPlayer()
    {
        if (Vector3.Distance(transform.position, _player.transform.position) < _searchingRange && _fsm.CurrentState != _chaseState)
        {
            _fsm.CurrentState = _chaseState;
        }
    }
    private void ChasingPlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, _chaseMoveSpeed * Time.deltaTime);
    }
    private void ChaseFailed()
    {
        /*
        if(_fsm.CurrentState == _chaseState && )
        {

        }
        */
    }
    private void HandleState()
    {
        _fsm.Update();

        if (Input.GetKeyDown("1"))
        {
            _fsm.CurrentState = _idleState;
        }
        if (Input.GetKeyDown("2"))
        {
            _fsm.CurrentState = _roamState;
        }
        if (Input.GetKeyDown("3"))
        {
            _fsm.CurrentState = _chaseState;
        }
        if (Input.GetKeyDown("4"))
        {
            _fsm.CurrentState = _returnState;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject == _player.gameObject)
        {
            _isPlayerInSight = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject == _player.gameObject)
        {
            _isPlayerInSight = false; 
        }
    }
}
