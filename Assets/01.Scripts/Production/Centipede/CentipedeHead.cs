using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeHead : MonoBehaviour
{
    [SerializeField] private AudioClip _movingSound;
    [SerializeField] private float _volume, _pitch;

    [SerializeField] private GameObject _bodyPrefab;
    [SerializeField] private GameObject _tailPrefab;

    [SerializeField] private int _bodyCount = 7;
    [SerializeField] private float _bodyDistance = 0.3f;

    private readonly List<GameObject> _children = new();
    private Vector3 _beforePos;

    private float _targetAngle;

    private SFXController _moveSoundController;
    private float _moveSoundTimer = 0f;

    private void Awake()
    {
        for(int i = 0; i < _bodyCount; i++)
        {
            var body = Instantiate(_bodyPrefab, transform.position, 
                Quaternion.identity);
            _children.Add(body);
        }
        _children.Add(Instantiate(_tailPrefab, transform.position,
            Quaternion.identity));
        _beforePos = transform.position;
        _targetAngle = transform.eulerAngles.z;

        foreach(var child in _children)
        {
            child.transform.localScale = transform.localScale;
        }
    }

    private void Start()
    {
        _moveSoundController = SoundManager.Instance.PlayLoopSFX(_movingSound, transform.position, 0f, _pitch, transform);
    }

    private void Update() 
    {
        var headDir = transform.position - _beforePos;
        if(headDir.sqrMagnitude > 0.3f * 0.3f)
        {
            _targetAngle = Mathf.Atan2(headDir.y, headDir.x) * Mathf.Rad2Deg - 90f;
            _beforePos = transform.position;
            _moveSoundTimer = 0.5f;
        }
        transform.eulerAngles = new(0, 0, Mathf.MoveTowardsAngle(transform.eulerAngles.z, _targetAngle, Time.deltaTime * 360f));

        if(_moveSoundTimer > 0f)
        {
            _moveSoundTimer -= Time.deltaTime;
            _moveSoundController.Volume = _volume;
        }
        else
        {
            _moveSoundController.Volume = 0;
        }

        var before = gameObject;
        foreach(var child in _children)
        {
            var distance = (before.transform.position - child.transform.position);
            child.transform.up = distance.normalized;
            child.transform.position = before.transform.position + Mathf.Clamp(distance.magnitude, 0, _bodyDistance * transform.localScale.x) * -child.transform.up;
            before = child;
        }
    }
}
