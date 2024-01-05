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

    Rigidbody2D _rigidbody;

    //몬스터가 배치된 위치
    private Vector2 _initPosition;
    private Vector2 _moveDistance;
    private Vector3 _destination;
    private bool _IsSettedInitPosition;

    private GameObject _screamParticle;

    // Start is called before the first frame update
    void Start()
    {
        _IsSettedInitPosition = false;
        _rigidbody = GetComponent<Rigidbody2D>();

        _screamParticle = transform.GetChild(0).gameObject;
        _screamParticle.SetActive(false);

        CrySounds.Add(Crying1);
        CrySounds.Add(Crying2);
        CrySounds.Add(Crying3);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            SetDestination();
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            StartCoroutine(PlayCryingSound());
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            StartCoroutine(EncountWithPlayer());
        }


    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_IsSettedInitPosition) SetInitPosition();

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
        if (Mathf.Abs(_initPosition.x - transform.position.x) > _maxDistance) ReturnToInitPosition();

        StartCoroutine(Moving());
    }
    private IEnumerator Moving()
    {
        while (_destination != transform.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, _destination, _moveSpeed * Time.deltaTime);

            yield return null;
        }
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
        yield return new WaitForSeconds(0.7f);
        _screamParticle.SetActive(true);
        yield return new WaitForSeconds(4);
        _screamParticle.SetActive(false);
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
