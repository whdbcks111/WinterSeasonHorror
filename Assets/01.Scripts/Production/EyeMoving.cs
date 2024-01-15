using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeMoving : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _pupil;
    [SerializeField] private Animator _skinAnimator;
    [SerializeField] private float _maxDistance;
    [SerializeField] private float _detectRange;
    [SerializeField] private float _smoothTime;

    private bool _isClosed = true;

    private Vector3 _vel = Vector3.zero;

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
        }

        if (!_isClosed && (Player.Instance.transform.position.x < transform.position.x) != Player.Instance.IsLeftDir)
        {
            _isClosed = true;
            _skinAnimator.SetTrigger("Close");
        }
    }
}
