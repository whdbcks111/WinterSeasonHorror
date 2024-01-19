using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    [SerializeField] private SpriteRenderer _reflectionSpriteRenderer;

    [Header("포스트 프로세싱 볼륨 할당")]
    [SerializeField] private Volume _postProcessingVolume;

    [Header("콜라이더(충돌체) 할당")]
    [SerializeField] private Collider2D _feetCollider;
    [SerializeField] private Collider2D _bodyCollider;

    [Header("라이트 할당")]
    [SerializeField] private Light2D _handLight;
    [SerializeField] private Light2D _surroundLight;

    [Header("상호작용 아이콘")]
    [SerializeField] private SpriteRenderer _bangMark;

    [Header("카메라 오프셋")]
    [SerializeField] private Vector2 _camOffset;

    [Header("숨었을 때 색상")]
    [SerializeField] private Color _hiddenColor;
    [Header("숨었을 때 레이어 우선순위")]
    [SerializeField] private int _hiddenOrderInLayer = -50;
    [Header("숨고 나서 다시 숨기까지 쿨타임")]
    [SerializeField] private float _hideCooldown = 3f;

    [Header("플레이어 이동속도")]
    [SerializeField] private float _moveSpeed = 5;
    [Header("플레이어 방향전환 시간")]
    [SerializeField] private float _moveShiftTime;
    [Header("플레이어 달리기 방향전환 관성")]
    [SerializeField] private float _runShiftInertia = 3f;
    [Header("플레이어 달리기 배속")]
    [SerializeField] private float _runModifier = 1.3f;
    [Header("플레이어 일반 점프력")]
    [SerializeField] private float _jumpForce = 5;
    [Header("플레이어 장애물 넘는 점프력")]
    [SerializeField] private float _highJumpForce = 10;
    [Header("플레이어 점프 가능 횟수")]
    [SerializeField] private int _maxJumpCount = 1;

    [Header("플레이어 발자국 간격 (초)")]
    [SerializeField] private float _footStepAudioSpan = 1f;

    [Header("플레이어 손전등 전력 지속시간 (초)")]
    public float MaxLightEnerge = 100;
    [HideInInspector] public float LightEnerge;

    [Header("플레이어 달리기 지속시간 (초)")]
    public float MaxStamina = 20f;
    [HideInInspector] public float Stamina;

    [Header("효과음 할당")]
    [SerializeField] private AudioClip _footStepClip;
    [SerializeField] private float _footStepVolume, _footStepPitch, _footStepPitchRandom;
    [SerializeField] private AudioClip _jumpClip;
    [SerializeField] private float _jumpVolume;
    [SerializeField] private AudioMixerGroup _breatheGroup;
    [SerializeField] private AudioClip _breatheClip;
    [SerializeField] private float _breathePitch = 1f, _breatheMinVolume = 0.1f, 
        _breatheMaxVolume = 2f, _breatheMinSpeed = 0.5f, _breatheMaxSpeed = 2f;
    [SerializeField] private AudioClip _heartbeatClip;
    [SerializeField] private float _heartbeatPitch = 1f, _heartbeatMinVolume = 0.1f, _heartbeatMaxVolume = 2f;

    [Header("거친 숨 스크린")]
    [SerializeField] private Image _breatheScreenImage;
    [SerializeField] private float _maxBreatheScreenAlpha = 1f;
    [SerializeField] private float _breatheScreenBeatRate = 1f;
    [SerializeField] private float _breatheScreenBeatScale = 1.5f;

    private Rigidbody2D _rigid;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    private readonly HashSet<Collider2D> _steppingGrounds = new();
    private bool _isLeftDir = false;
    private bool _isShifting = false;
    private bool _isRunning = false;
    private float _runningTime = 0f, _stoppingTime = 0f;

    private int _jumpCount = 0;
    private bool _isJumping = false;
    private bool _isLeftJump = false;
    private float _moveShiftTimer = 0f;

    private bool _isLighting;
    private float _handLightDefaultAngleX;
    private float _handLightOffsetX, _surroundLightOffsetX;

    private bool _isHidden = false;
    private int _defaultOrderInLayer;
    private float _hideTimer = 0f;
    private float _hideCannotMoveTimer = 0f;

    private bool _isControllable = true;
    private float _footStepAudioTimer = 0f;

    private bool _isDead = false;

    private float _lightKeyHoldingTimer = 0f;

    private SFXController _breatheController, _heartbeatController;

    private readonly Dictionary<string, Action> _onFootStepListeners = new();

    public Vector2 Velocity { get => _rigid.velocity;  }
    public float FeetPositionY { get => _feetCollider.bounds.min.y; }
    public bool IsControllable { get => _isControllable; set => _isControllable = value; }
    public bool IsLeftDir { get => _isLeftDir; }
    public bool IsHidden { get => _isHidden; }
    public float LightHoldingTime { get => _lightKeyHoldingTimer; }
    public bool IsInHideCooldown { get => _hideTimer > 0f; }
    public bool IsLightOn { get => _handLight.gameObject.activeSelf; }
    public bool IsOnGround { get => _steppingGrounds.Count > 0; }
    public float MoveSpeed { get => _moveSpeed; }
    public bool BangMarkVisible 
    { 
        get => _bangMark.gameObject.activeSelf; 
        set => _bangMark.gameObject.SetActive(value);
    }
    [HideInInspector] public bool IsInBlind = false;
    [HideInInspector] public bool IsMoveable = true;

    private void Awake()
    {
        Instance = this;
        LightEnerge = MaxLightEnerge;
        Stamina = MaxStamina;
        _handLightDefaultAngleX = _handLight.transform.eulerAngles.z;
        _rigid = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _defaultOrderInLayer = _spriteRenderer.sortingOrder;
        _isLighting = _handLight.gameObject.activeSelf;
        _handLightOffsetX = _handLight.transform.position.x - transform.position.x;
        _surroundLightOffsetX = _surroundLight.transform.position.x - transform.position.x;
    }

    private void Start()
    {
        CameraController.Instance.SetFocus(transform, 0.5f);
        _breatheController = SoundManager.Instance.PlayLoopSFX(_breatheClip, transform.position,
            _breatheMaxVolume, _breatheMaxSpeed, transform, _breatheGroup);
        _heartbeatController = SoundManager.Instance.PlayLoopSFX(_heartbeatClip, transform.position,
            _heartbeatMaxVolume, _breatheMaxSpeed, transform, _breatheGroup);
    }

    private void Update()
    {
        JumpUpdate();
        LightUpdate();
        MoveUpdate();
        MoveShiftUpdate();
        HiddenViewUpdate();
        BreatheUpdate();
        ReflectUpdate();
    }

    private void OnDeath()
    {
        UIManager.Instance.PlayJumpScare(UIManager.Instance.jumpScare, 
            onFinish: () =>
            {
                SceneManager.LoadScene("GameOverScene");
            });
    }

    private void ReflectUpdate()
    {
        _reflectionSpriteRenderer.sprite = _spriteRenderer.sprite;
        _reflectionSpriteRenderer.flipX = _spriteRenderer.flipX;
    }

    private void BreatheUpdate()
    {
        const float blurThreshold = 0.7f, screenPanelThreshold = 0.3f;
        var progress = 1 - Stamina / MaxStamina;
        _breatheController.Pitch = Mathf.Lerp(_breatheMinSpeed, _breatheMaxSpeed, progress) * _breathePitch;
        _breatheController.Volume = Mathf.Lerp(_breatheMinVolume, _breatheMaxVolume, progress);
        _heartbeatController.Pitch = Mathf.Lerp(_breatheMinSpeed, _breatheMaxSpeed, progress) * _heartbeatPitch;
        _heartbeatController.Volume = Mathf.Lerp(_heartbeatMinVolume, _heartbeatMaxVolume, progress);
        _breatheGroup.audioMixer.SetFloat("BreathePitch", 1 / _breatheController.Pitch);

        var breatheScreenColor = _breatheScreenImage.color;
        if (progress < screenPanelThreshold)
        {
            breatheScreenColor.a = 0f;
        }
        else
        {
            var screenProgress = (progress - screenPanelThreshold) / (1 - screenPanelThreshold);
            breatheScreenColor.a = Mathf.Clamp01(
                Mathf.Lerp(0f, _maxBreatheScreenAlpha, screenProgress) 
                * (Mathf.Sin(Time.time * Mathf.PI * _breatheScreenBeatRate) + 2f) * 0.5f * _breatheScreenBeatScale
                );
        }
        _breatheScreenImage.color = breatheScreenColor;

        if (_postProcessingVolume.profile.TryGet<DepthOfField>(out var depthOfField))
        {
            if(progress < blurThreshold)
            {
                depthOfField.mode.Override(DepthOfFieldMode.Off);
            }
            else
            {
                var blurProgress = (progress - blurThreshold) / (1 - blurThreshold);
                depthOfField.mode.Override(DepthOfFieldMode.Gaussian);

                depthOfField.gaussianMaxRadius.Override(Mathf.Lerp(0.5f, 1.5f, blurProgress));
            }
        }
    }

    public void Hide()
    {
        if (_isHidden) return;
        if (_hideTimer > 0f) return;
        _isHidden = true;
        _hideCannotMoveTimer = 0.5f;
    }

    public void Reveal() {
        if (!_isHidden) return;
        _isHidden = false;
        _hideTimer = _hideCooldown;
    }

    public void AddFootStepListener(string key, Action listener)
    {
        _onFootStepListeners[key] = listener;
    }

    public void RemoveFootStepListener(string key)
    {
        _onFootStepListeners.Remove(key);
    }

    private void HiddenViewUpdate()
    {
        if(_hideTimer > 0f)
            _hideTimer -= Time.deltaTime;
        if(_hideCannotMoveTimer > 0f)
            _hideCannotMoveTimer -= Time.deltaTime;

        _spriteRenderer.sortingOrder = IsHidden ? _hiddenOrderInLayer : _defaultOrderInLayer;
        _spriteRenderer.color = IsHidden ? _hiddenColor : Color.white;
    }

    private void StartMoveShift(bool isRunning)
    {
        if(_runningTime > 1f) _rigid.velocity = (_isLeftDir ? 1 : -1) * _runShiftInertia * Vector3.left;
        _isShifting = true;
        _moveShiftTimer = _moveShiftTime;
        _animator.SetBool(isRunning ? "IsRunShifting" : "IsWalkShifting", true);
    }

    private void MoveShiftUpdate()
    {
        _animator.SetBool("IsShifting", _isShifting);
        if (!_isShifting) return;
        if(_moveShiftTimer > 0)
        {
            _moveShiftTimer -= Time.deltaTime;
        }
        else
        {
            _isShifting = false;
            _isLeftDir = !_isLeftDir;

            _animator.SetBool("IsRunShifting", false);
            _animator.SetBool("IsWalkShifting", false);
            _spriteRenderer.flipX = _isLeftDir;
        }
    }

    private void MoveUpdate()
    {
        // 방향전환중이면 _isLeftDir의 반대로 미리 이동
        CameraController.Instance.SetOffset(_camOffset * new Vector2(_isLeftDir == _isShifting ? 1 : -1, 1), _moveShiftTime);
        if (_isShifting) return;
        if (_hideCannotMoveTimer > 0 || !IsMoveable)
        {
            _rigid.velocity = new(0, _rigid.velocity.y);
            return;
        }

        var xAxis = Input.GetAxisRaw("Horizontal");
        if (!IsControllable || 
            Stamina < 0f ||
            (_isJumping && (xAxis > 0f && _isLeftJump || xAxis < 0f && !_isLeftJump)))
        {
            xAxis = 0f; 
        }

        var nextIsLeftDir = xAxis < 0f;
        var isMoving = !Mathf.Approximately(xAxis, 0f);
        var isInRunningKey = IsControllable && Input.GetKey(KeyCode.LeftShift);
        var nextIsRunning = isInRunningKey && Stamina > 0f && isMoving;

        if(_isRunning != nextIsRunning)
        {
            _footStepAudioTimer = 0f;
        }
        _isRunning = nextIsRunning;

        if(_isRunning)
        {
            _runningTime += Time.deltaTime;
            _stoppingTime = 0f;
        }
        else
        {
            _stoppingTime += Time.deltaTime;
            if(_stoppingTime > 0.5f) _runningTime = 0f;
        }
        

        if (isMoving)
        {
            // moving
            if(_isLeftDir != nextIsLeftDir)
            {
                StartMoveShift(_isRunning);
                return;
            }
            Reveal();

            if (_footStepAudioTimer > 0f)
            {
                _footStepAudioTimer -= Time.deltaTime;
            }
            else if(IsOnGround)
            {
                SoundManager.Instance.PlaySFX(_footStepClip, transform.position, _footStepVolume, 
                    _footStepPitch + UnityEngine.Random.Range(0f, _footStepPitchRandom), transform);
                _footStepAudioTimer = _footStepAudioSpan / (_isRunning ? _runModifier : 1f);

                foreach(var listener in _onFootStepListeners)
                {
                    listener.Value?.Invoke();
                }
            }

            var raycastResult = Physics2D.Raycast(transform.position + Vector3.up * 0.3f, Vector3.right * xAxis, 1f, LayerMask.GetMask("Pushable"));

            _animator.SetBool("IsPushing", raycastResult.collider != null);
        }

        var velX = xAxis * _moveSpeed;

        if (_isRunning)
        {
            velX *= _runModifier;
            Stamina -= Time.deltaTime;

            if (Stamina <= 0f)
            {
                Stamina = -5f;
            }
        }
        else if (Stamina < MaxStamina)
        {
            Stamina += Time.deltaTime;
        }


        _rigid.velocity = new(velX, _rigid.velocity.y);

        _animator.SetBool("IsRunning", _isRunning && !_isShifting);
        _animator.SetBool("IsResting", Stamina <= 0f);
        _animator.SetBool("IsWalking", isMoving && !_isShifting);
    }


    private void LightUpdate()
    {
        const KeyCode LightKey = KeyCode.W;

        if(IsControllable && Input.GetKeyUp(LightKey) && _lightKeyHoldingTimer < 1f)
        {
            _isLighting = !_isLighting;
        }
        _handLight.gameObject.SetActive(_isLighting && !IsInBlind && LightEnerge > 0f && !_isShifting && Stamina > 0f);
        _surroundLight.gameObject.SetActive(!IsInBlind);

        if (Input.GetKey(LightKey))
        {
            _lightKeyHoldingTimer += Time.deltaTime;
        }
        else _lightKeyHoldingTimer = 0f;

        if (IsLightOn) LightEnerge -= Time.deltaTime;

        int dir = _isLeftDir ? -1 : 1;
        _handLight.transform.localPosition = new(dir * _handLightOffsetX, _handLight.transform.localPosition.y);
        _surroundLight.transform.localPosition = Vector3.MoveTowards(
            _surroundLight.transform.localPosition, 
            new(dir * _surroundLightOffsetX, _surroundLight.transform.localPosition.y),
            Time.deltaTime);
        _handLight.transform.eulerAngles = new(0, 0, dir * _handLightDefaultAngleX);
    }

    private void JumpUpdate()
    {
        if (IsOnGround && _rigid.velocity.y <= 0.01f) _isJumping = false;

        if (IsControllable && Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        _animator.SetBool("IsJumping", _isJumping);
        _animator.SetBool("IsFalling", _rigid.velocity.y < -0.01f);
    }

    public void Jump()
    {
        if (_hideCannotMoveTimer > 0f || IsHidden || !IsMoveable || Stamina < 0f)
        {
            return;
        }
        if (IsOnGround && _jumpCount > 0)
        {
            _isJumping = true;
            _jumpCount--;
            _isLeftJump = _isLeftDir;

            var raycastResult = Physics2D.Raycast(
                _bodyCollider.bounds.center, Vector3.right * (_isLeftDir ? -1 : 1), 1.2f, LayerMask.GetMask("Pushable", "Wall"));
            _rigid.velocity = new(_rigid.velocity.x, raycastResult.collider != null || _isRunning ? _highJumpForce : _jumpForce);

            SoundManager.Instance.PlaySFX(_jumpClip, transform.position, _jumpVolume, 1f);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        for(int i = 0; i < collision.contactCount; i++)
        {
            if (_feetCollider.bounds.min.y + 0.1f >= collision.GetContact(i).point.y &&
                _rigid.velocity.y <= 0.01f)
            {
                if(!_steppingGrounds.Contains(collision.collider))
                    _steppingGrounds.Add(collision.collider);
                _jumpCount = _maxJumpCount;
                break;
            }

        }

        if(!_isDead && collision.collider.TryGetComponent(out Believer _))
        {
            _isDead = true;
            OnDeath();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (_steppingGrounds.Contains(collision.collider))
        {
            _steppingGrounds.Remove(collision.collider);
        }
    }
}
