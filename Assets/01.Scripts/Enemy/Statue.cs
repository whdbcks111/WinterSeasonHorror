using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Statue : MonoBehaviour
{
    Believer _believer;
    Player _player;
    [Header("�� ��׷� ����. ���� ������ ��")]
    [SerializeField] private Vector2 _alarmBoxSize;
    [Header("�� ��׷� ������ ��ġ. ���̾��Ű���� ã�� ��")]
    [SerializeField] private Transform _alarmBoxPosition;
    [Header("�÷��̾� ���� ������ ��ġ. ���̾��Ű���� ã�� ��")]
    [SerializeField] private Transform _searchBoxPosition;

    [Header("���� ��� �����ð�(��)")]
    [SerializeField] private float _maxSecond;
    private float _currentSecond;

    [Header("����� �ܾ� ������")]
    [SerializeField] GameObject[] _statuePresets;
    private GameObject _selectedStatue;
    [Header("����� �� ������")]
    [SerializeField] GameObject[] _eyePresets;
    private GameObject _selectedEyePreset;

    private bool _canOrder;



    [SerializeField] GameObject _statuePreview;



    BoxCollider2D _boxCollider;
    private bool _playerIsInRange;

    private Vector3 _velocity = Vector3.zero;
    private void Start()
    {
        _player = Player.Instance;

        _canOrder = true;
        _selectedStatue = SelectPreset();
        _statuePreview.SetActive(false);
        _boxCollider = GetComponent<BoxCollider2D>();
    }
    void Update()
    {
        

        if (Input.GetKeyDown(KeyCode.V))
        {
            AttackOrder();
        }

        if (_playerIsInRange && !Player.Instance.IsHidden)
        {
            if ((transform.position.x > Player.Instance.transform.position.x) == Player.Instance.IsLeftDir) //�÷��̾ ������ ����
            {
                ActivateEye();
                if((_currentSecond >= _maxSecond || Player.Instance.LightEnerge <= 0) && _canOrder)
                {
                    AttackOrder();
                }
            }
            else DeactivateEye();
        }
        else DeactivateEye();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(_alarmBoxPosition.position, _alarmBoxSize);
        //Gizmos.DrawWireCube(_searchBoxPosition.position, _boxCollider.size);
    }
    private void AttackOrder()
    {
        RaycastHit2D[] hits = Physics2D.BoxCastAll(_alarmBoxPosition.position, _alarmBoxSize,0,Vector2.zero);
        foreach(RaycastHit2D hit in hits)
        {
            if(hit.collider.gameObject.TryGetComponent<Believer>(out Believer BelieverScript) && BelieverScript._fsm.CurrentState != BelieverScript._threteningHide && BelieverScript._fsm.CurrentState != BelieverScript._chaseState)
            {
                BelieverScript._fsm.CurrentState = BelieverScript._chaseState;
            }
        }

        _canOrder = false;
    }
    void ActivateEye()
    {
        _selectedEyePreset.SetActive(true);
        _currentSecond += Time.deltaTime;

        _canOrder = true;
    }
    void DeactivateEye()
    {
        _selectedEyePreset.SetActive(false);

        _currentSecond = 0;

        _canOrder = false;
    }
    private GameObject SelectPreset()
    {
        int Selecter = Random.Range(0, _statuePresets.Length);
        GameObject SelectedPreset = Instantiate(_statuePresets[Selecter],transform.position,Quaternion.identity);
        _selectedEyePreset = Instantiate(_eyePresets[Selecter],transform.position,Quaternion.identity);

        SelectedPreset.transform.parent = gameObject.transform;
        _selectedEyePreset.transform.parent = gameObject.transform;


        /*
        switch (Selecter) {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            default:
                break;
        }
        *///����ġ
        return SelectedPreset;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject == Player.Instance.gameObject)
        {
            _playerIsInRange = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == Player.Instance.gameObject)
        {
            _playerIsInRange = false;
        }
    }
}
