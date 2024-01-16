using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Statue : MonoBehaviour
{
    public float radius = 1.0f;
    public float maxDistance = 5.0f;


    Believer _believer;
    Player _player;
    [SerializeField] private Vector2 _alarmBoxSize;
    [SerializeField] private Transform _alarmBoxPosition;
    [SerializeField] private Transform _searchBoxPosition;

    [Header("수녀상 외양 프리셋")]
    [SerializeField] GameObject[] _statuePresets;
    private GameObject _selectedStatue;
    [Header("수녀상 눈 프리셋")]
    [SerializeField] GameObject[] _eyePresets;
    private GameObject _selectedEyePreset;

    [SerializeField] GameObject _statuePreview;



    BoxCollider2D _boxCollider;
    private bool _playerIsInRange;

    private Vector3 _velocity = Vector3.zero;
    private void Start()
    {
        _player = Player.Instance;

        _selectedStatue = SelectPreset();
        _statuePreview.SetActive(false);
        _boxCollider = GetComponent<BoxCollider2D>();
    }
    void Update()
    {
        _boxCollider.offset = new Vector2(_searchBoxPosition.localPosition.x,_searchBoxPosition.localPosition.y);

        if (Input.GetKeyDown(KeyCode.V))
        {
            AttackOrder();
        }

        if (_playerIsInRange)
        {
            if ((transform.position.x > Player.Instance.transform.position.x) == Player.Instance.IsLeftDir) //플레이어가 등지고 있음
            {
                ActivateEye();
            }
            if ((transform.position.x > Player.Instance.transform.position.x) != Player.Instance.IsLeftDir) //플레이어가 바라보고 있음
            {
                DeactivateEye();
            }
        }
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
    }
    void ActivateEye()
    {
        _selectedEyePreset.SetActive(true);
    }
    void DeactivateEye()
    {
        _selectedEyePreset.SetActive(false);
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
        *///스위치
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
