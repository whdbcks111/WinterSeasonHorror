using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EyeMoving : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _pupil;
    [SerializeField] private Animator _skinAnimator;
    [SerializeField] private float _maxDistance;
    [SerializeField] private float _detectRange;
    [SerializeField] private float _smoothTime;
    [SerializeField] private bool _haveEyelight;

    private bool _isClosed = true;
    private Light2D _eyeLt; 

    private Vector3 _vel = Vector3.zero;

    private void Start()
    {
        if(_haveEyelight)
            _eyeLt = GetComponentInChildren<Light2D>();
    }
    private void Update()
    {
        var playerDir = (Player.Instance.transform.position - transform.position);
        var targetPos = playerDir.normalized * Mathf.Clamp(playerDir.magnitude, 0, _maxDistance);

        _pupil.transform.localPosition = Vector3.SmoothDamp(_pupil.transform.localPosition,
            targetPos,
            ref _vel, _smoothTime);

        if (_isClosed && (Player.Instance.transform.position.x < transform.position.x) == Player.Instance.IsLeftDir)
        {
            _isClosed = false;
            _skinAnimator.SetTrigger("Open");
            if (_haveEyelight)
                _eyeLt.enabled = true;

        }

        if (!_isClosed && (Player.Instance.transform.position.x < transform.position.x) != Player.Instance.IsLeftDir)
        {
            _isClosed = true;
            _skinAnimator.SetTrigger("Close");
            if (_haveEyelight)
                _eyeLt.enabled = false;
        }
    }
    }

