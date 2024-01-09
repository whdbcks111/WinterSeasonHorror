using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [SerializeField] private Camera _camera;
    [SerializeField] private float _defaultSmoothTime;
    [SerializeField] private Material _glitchMaterial;

    private Vector3 _curCameraPos;
    private float _smoothTime;
    private Vector2 _camOffset;
    private Vector2 _staticPos;
    private bool _isStatic;
    private Transform _focusTarget;
    private Vector3 _vel = Vector3.zero;

    private float _zoomSize, _zoomSmoothTime = 0f;
    private float _zoomVel = 0f;

    private float _glitchStrength = 0f, _targetGlitchStrength = 0f;
    private float _glitchSmoothTime = 0f, _glitchVel = 0f;

    private float _cameraShakeOffset = 0f, _cameraShakeTimer = 0f;

    public float ZoomSize { get => _zoomSize; }
    public float GlitchStrength { get => _targetGlitchStrength; }

    private void Awake()
    {
        Instance = this;
        _zoomSize = _camera.orthographicSize;
        _curCameraPos = transform.position;
    }

    private void LateUpdate()
    {
        Vector2 target = _isStatic && _focusTarget != null ? _staticPos : (Vector2)_focusTarget.position + _camOffset;

        _curCameraPos = Vector3.SmoothDamp(
            _curCameraPos, 
            new Vector3(target.x, target.y, transform.position.z), 
            ref _vel, _smoothTime);
        transform.position = _curCameraPos;
        if(_cameraShakeTimer > 0f)
        {
            transform.position += new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * _cameraShakeOffset;
            _cameraShakeTimer -= Time.deltaTime;
        }
        _camera.orthographicSize = Mathf.SmoothDamp(_camera.orthographicSize, _zoomSize, ref _zoomVel, _zoomSmoothTime);

        _glitchStrength = Mathf.SmoothDamp(_glitchStrength, _targetGlitchStrength, ref _glitchVel, _glitchSmoothTime);
        _glitchMaterial.SetFloat("_Intensity", Mathf.Clamp(_glitchStrength * 0.8f, 0, 10));
        _glitchMaterial.SetFloat("_Threshold", Mathf.Clamp01(_glitchStrength * 0.1f));
        _glitchMaterial.SetFloat("_Offset", Mathf.Clamp01(_glitchStrength * 5f));
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

    public void Shake(float offset, float time)
    {
        _cameraShakeTimer = time;
        _cameraShakeOffset = offset;
    }

    public void SetGlitch(float strength, float time)
    {
        _targetGlitchStrength = strength;
        _glitchSmoothTime = time;
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
