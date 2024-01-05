using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    [Header("���� �̵� �ӵ�")]
    [SerializeField] private float _moveSpeed;
    [Header("���� �̵� �Ÿ� ����(���� �� ������)")]
    [SerializeField] private float _moveRange;
    [Header("���� �̵� �� �ּ� �̵��Ÿ�")]
    [SerializeField] private float _minMoveDistance;
    [Header("���� �ִ� �̵� �Ÿ�(�� �� ������ ó����ġ��)")]
    [SerializeField] private float _maxDistance;
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

    Rigidbody2D _rigidbody;

    //���Ͱ� ��ġ�� ��ġ
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

        //�̵� �Ÿ��� ���밪�� minMoveDistance �����϶� minMoveDistance��ŭ �����̵��� ��.
        if (_moveDistance.x <= _minMoveDistance && _moveDistance.x >= 0) _moveDistance.x = _minMoveDistance;
        if (_moveDistance.x >= -_minMoveDistance && _moveDistance.x <= 0) _moveDistance.x = -_minMoveDistance;

        _destination = new Vector3(_moveDistance.x, transform.position.y, 0);

        //�ʱ� ��ġ���� �ʹ� �־����� ��
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
