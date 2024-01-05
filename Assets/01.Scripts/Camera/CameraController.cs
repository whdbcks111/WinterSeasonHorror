using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [SerializeField] private Camera _camera;
    [SerializeField] private float _defaultSmoothTime;

    private float _smoothTime;
    private Vector2 _camOffset;
    private Vector2 _staticPos;
    private bool _isStatic;
    private Transform _focusTarget;

    private Vector3 _vel;
    private float _zoomVel;

    private float _zoomSize, _zoomSmoothTime = 0f;

    public float ZoomSize { get => _zoomSize; }

    private void Awake()
    {
        _zoomSize = _camera.orthographicSize;
        Instance = this;
    }

    private void LateUpdate()
    {
        Vector2 target = _isStatic && _focusTarget != null ? _staticPos : (Vector2)_focusTarget.position + _camOffset;

        transform.position = Vector3.SmoothDamp(transform.position, new Vector3(target.x, target.y, transform.position.z), ref _vel, _smoothTime);
        _camera.orthographicSize = Mathf.SmoothDamp(_camera.orthographicSize, _zoomSize, ref _zoomVel, _zoomSmoothTime);
    }

    public void SetStatic(Vector2 pos, float time = -1)
    {
        if (time < 0) time = _defaultSmoothTime;
        if (_staticPos == pos) return;
        _isStatic = true;
        _staticPos = pos;
        _smoothTime = time;
    }

    public void Zoom(float zoom, float time)
    {
        _zoomSize = zoom;
        _zoomSmoothTime = time;
    }

    public void SetFocus(Transform focusTarget, float time = -1)
    {
        if (time < 0) time = _defaultSmoothTime;
        if (_focusTarget == focusTarget) return;
        _isStatic = false;
        _focusTarget = focusTarget;
        _smoothTime = time;
    }

    public void SetOffset(Vector2 offset, float time = -1)
    {
        if (time < 0) time = _defaultSmoothTime;
        if (offset == _camOffset) return;
        _camOffset = offset;
        _smoothTime = time;
    }
}
