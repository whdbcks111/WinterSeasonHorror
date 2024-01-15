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
    [SerializeField] private Vector2 _searchBoxSize;
    [SerializeField] private Transform _searchBoxPosition;

    [SerializeField] private GameObject _eyeSphere;
    [SerializeField] private GameObject _pupil;

    [SerializeField] private float _eyeMaxDistance;
    [SerializeField] private float _smoothTime;

    [Header("수녀상 종류")]
    [SerializeField] GameObject[] _statuePresets;
    private GameObject _selectedStatue;
    [SerializeField] GameObject _statuePreview;

    private bool _playerIsInRange;

    private Vector3 _velocity = Vector3.zero;
    private void Start()
    {
        _player = Player.Instance;

        _selectedStatue = SelectPreset();
        _statuePreview.SetActive(false);
        
        _pupil.SetActive(false);
        
    }
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.V))
        {
            AttackOrder();
        }
        SearchNear();

        /*
        if (_playerIsInRange && (transform.position.x > Player.Instance.transform.position.x) == Player.Instance.IsLeftDir)
        {
            //플레이어가 등지고 있음.
            ActivateEye();
            GazingPlayer();
        }
        if (_playerIsInRange && (transform.position.x > Player.Instance.transform.position.x) != Player.Instance.IsLeftDir)
        {
            //플레이어가 쳐다보고 있음.
            DeactivateEye();
        }
        */
        ActivateEye();
        GazingPlayer();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(_alarmBoxPosition.position, _alarmBoxSize);
        Gizmos.DrawWireCube(_searchBoxPosition.position, _searchBoxSize);
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
        _eyeSphere.SetActive(true);
        _pupil.SetActive(true);
    }
    void DeactivateEye()
    {
        _eyeSphere.SetActive(false);
        _pupil.SetActive(false);
    }
    void GazingPlayer()
    {
        var Direction = (Player.Instance.transform.position - _eyeSphere.transform.position);
        var PupilPos = Direction.normalized * Mathf.Clamp(Direction.magnitude, 0, _eyeMaxDistance);

        Debug.Log(PupilPos);
        _pupil.transform.localPosition = Vector3.SmoothDamp(_pupil.transform.localPosition, PupilPos, ref _velocity, _smoothTime);
    }
    void SearchNear()
    {
        RaycastHit2D[] hits = Physics2D.BoxCastAll(_searchBoxPosition.position, _searchBoxSize, 0, Vector2.zero);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject == Player.Instance.gameObject)
            {
                _playerIsInRange = true;
            }
            else _playerIsInRange = false;
        }
    }
    private GameObject SelectPreset()
    {
        int Selecter = Random.Range(0, _statuePresets.Length);
        GameObject SelectedPreset = Instantiate(_statuePresets[Selecter],transform.position,Quaternion.identity);
        SelectedPreset.transform.parent = gameObject.transform;

        _pupil = Instantiate(_pupil, transform.position, Quaternion.identity);
        _eyeSphere = Instantiate(_eyeSphere, transform.position, Quaternion.identity);

        _pupil.transform.parent = gameObject.transform;
        _eyeSphere.transform.parent = gameObject.transform;

        switch (Selecter) {
            case 0:
                _pupil.transform.position = new Vector2(-0.139f, 0.265f) + new Vector2(transform.position.x,transform.position.y);
                _eyeSphere.transform.position = new Vector2(-0.139f, 0.265f) + new Vector2(transform.position.x, transform.position.y);
                break;
            case 1:
                _pupil.transform.position = new Vector2(-0.142f, 0.133f) + new Vector2(transform.position.x, transform.position.y);
                _eyeSphere.transform.position = new Vector2(-0.142f, 0.133f) + new Vector2(transform.position.x, transform.position.y);
                break;
            case 2:
                _pupil.transform.position = new Vector2(-0.343f, 0.095f) + new Vector2(transform.position.x, transform.position.y);
                _eyeSphere.transform.position = new Vector2(-0.343f, 0.095f) + new Vector2(transform.position.x, transform.position.y);
                break;
            case 3:
                _pupil.transform.position = new Vector2(-0.111f, 0.306f) + new Vector2(transform.position.x, transform.position.y);
                _eyeSphere.transform.position = new Vector2(-0.111f, 0.306f) + new Vector2(transform.position.x, transform.position.y);
                break;
            default:
                break;
        }


        return SelectedPreset;
    }
}
