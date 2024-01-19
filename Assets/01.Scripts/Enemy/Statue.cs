using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Statue : MonoBehaviour
{
    Believer _believer;
    Player _player;
    Light2D _light;

    [Header("적 어그로 범위. 직접 지정할 것")]
    [SerializeField] private Vector2 _alarmBoxSize;
    [Header("적 어그로 범위의 위치. 하이어라키에서 찾을 것")]
    [SerializeField] private Transform _alarmBoxPosition;
    [Header("플레이어 감지 범위의 위치. 하이어라키에서 찾을 것")]
    [SerializeField] private Transform _searchBoxPosition;

    [Header("공격 명령 유예시간(초)")]
    [SerializeField] private float _maxSecond;
    private float _currentSecond;

    [Header("수녀상 외양 프리셋")]
    [SerializeField] GameObject[] _statuePresets;
    private GameObject _selectedStatue;
    [Header("수녀상 눈 프리셋")]
    [SerializeField] GameObject[] _eyePresets;
    private GameObject _selectedEyePreset;

    private bool _canOrder;

    [SerializeField] float _tartgetIntensity;
    [SerializeField] float _targetRaduis;

    [SerializeField] float _intensitySmoothTime;
    [SerializeField] float _raduisSmoothTime;

    [SerializeField] GameObject _statuePreview;

    private float _vel;

    private bool _playerIsInRange;
    private bool _canChangeLight;
    private Vector3 _velocity = Vector3.zero;
    private void Start()
    {
        _player = Player.Instance;

        _canOrder = true;
        _canChangeLight = true;
        _selectedStatue = SelectPreset();
        _statuePreview.SetActive(false);
        _light = GetComponent<Light2D>();
    }
    void Update()
    {

        if (_playerIsInRange && !Player.Instance.IsHidden)
        {
            if ((transform.position.x > Player.Instance.transform.position.x) == Player.Instance.IsLeftDir) //플레이어가 등지고 있음
            {
                ActivateEye();
                if((_currentSecond >= _maxSecond || Player.Instance.LightEnerge <= 0))
                {
                    if(_canOrder)
                    {
                        AttackOrder();
                        StartCoroutine(Light());
                    }
                    if(_canChangeLight)
                    {
                        _light.intensity = Mathf.SmoothDamp(_light.intensity, _tartgetIntensity, ref _vel, _intensitySmoothTime);
                        _light.pointLightOuterRadius = Mathf.SmoothDamp(_light.pointLightOuterRadius, _targetRaduis, ref _vel, _raduisSmoothTime);
                    }
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
    

    private IEnumerator Light()
    {

        yield return new WaitForSeconds(1);
        _canChangeLight = false;
        while (_light.intensity >= 0)
        {
            _light.intensity = Mathf.SmoothDamp(_light.intensity, 0, ref _vel, _intensitySmoothTime);
            _light.pointLightOuterRadius = Mathf.SmoothDamp(_light.pointLightOuterRadius, 0, ref _vel, _raduisSmoothTime);
            yield return null;
        }
        _light.intensity = 0;
        _light.pointLightOuterRadius = 0;
    }
    void ActivateEye()
    {
        _selectedEyePreset.SetActive(true);
        _currentSecond += Time.deltaTime;

    }
    void DeactivateEye()
    {
        _selectedEyePreset.SetActive(false);

        _currentSecond = 0;

        _canOrder = true;
        _canChangeLight = true;
    }
    private GameObject SelectPreset()
    {
        int Selecter = Random.Range(0, _statuePresets.Length);
        GameObject SelectedPreset = Instantiate(_statuePresets[Selecter],transform.position,Quaternion.identity);
        _selectedEyePreset = Instantiate(_eyePresets[Selecter],transform.position,Quaternion.identity);

        SelectedPreset.transform.parent = gameObject.transform;
        _selectedEyePreset.transform.parent = gameObject.transform;

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
