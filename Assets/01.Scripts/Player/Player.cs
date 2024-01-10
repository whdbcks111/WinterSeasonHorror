using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    [SerializeField] private Collider2D _feetCollider, _bodyCollider;
    [SerializeField] private Light2D _handLight, _surroundLight;
    [SerializeField] private SpriteRenderer _bangMark;

    [SerializeField] private Vector2 _camOffset;

    [SerializeField] private Color _hiddenColor;
    [SerializeField] private float _moveSpeed = 5;
    [SerializeField] private float _moveShiftTime;
    [SerializeField] private float _runModifier = 1.3f;
    [SerializeField] private float _jumpForce = 5;
    [SerializeField] private float _highJumpForce = 10;
    [SerializeField] private int _maxJumpCount = 1;
    [SerializeField] private float _footStepAudioSpan = 1f;
    
    public float MaxLightEnerge = 100;
    [HideInInspector] public float LightEnerge;

    public float MaxStamina = 20f;
    [HideInInspector] public float Stamina;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip _footStepClip;
    [SerializeField] private float _footStepVolume, _footStepPitch, _footStepPitchRandom;
    [SerializeField] private AudioClip _jumpClip;
    [SerializeField] private float _jumpVolume;

    private Rigidbody2D _rigid;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    private readonly HashSet<Collider2D> _steppingGrounds = new();
    private bool _isLeftDir = false;
    private bool _isShifting = false;
    private bool _isRunning = false;

    private int _jumpCount = 0;
    private bool _isJumping = false;
    private bool _isLeftJump = false;
    private float _moveShiftTimer = 0f;

    private bool _isLighting;
    private float _handLightDefaultAngleX;
    private float _handLightOffsetX, _surroundLightOffsetX;

    private bool _isHidden = false;

    private float _footStepAudioTimer = 0f;

    private readonly Dictionary<string, Action> _onFootStepListeners = new();

    public bool IsLeftDir { get => _isLeftDir;  }
    public bool IsHidden { get => _isHidden; }
    public bool IsLightOn { get => _handLight.gameObject.activeSelf; }
    public bool IsOnGround { get => _steppingGrounds.Count > 0; }
    public float MoveSpeed { get => _moveSpeed; }
    public bool BangMarkVisible 
    { 
        get => _bangMark.gameObject.activeSelf; 
        set => _bangMark.gameObject.SetActive(value); 
    }

    private void Awake()
    {
        Instance = this;
        LightEnerge = MaxLightEnerge;
        Stamina = MaxStamina;
        _handLightDefaultAngleX = _handLight.transform.eulerAngles.z;
        _rigid = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _isLighting = _handLight.gameObject.activeSelf;
        _handLightOffsetX = _handLight.transform.position.x - transform.position.x;
        _surroundLightOffsetX = _surroundLight.transform.position.x - transform.position.x;
    }

    private void Start()
    {
        CameraController.Instance.SetFocus(transform, 0.5f);
    }

    private void Update()
    {
        JumpUpdate();
        LightUpdate();
        MoveUpdate();
        MoveShiftUpdate();
        HiddenViewUpdate();

        if(Input.GetKeyDown(KeyCode.S))
        {
            CameraController.Instance.Shake(0.1f, 1f);
        }
    }

    public void Hide() => _isHidden = true;
    public void Reveal() => _isHidden = false;

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
        _spriteRenderer.color = IsHidden ? _hiddenColor : Color.white;
    }

    private void StartMoveShift(bool isRunning)
    {
        if(isRunning) _rigid.AddForce((_isLeftDir ? 1 : -1) * 6 * Vector3.left, ForceMode2D.Impulse);
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

        var xAxis = Input.GetAxisRaw("Horizontal");
        if(_isJumping && (xAxis > 0f && _isLeftJump || xAxis < 0f && !_isLeftJump))
        {
            xAxis = 0f;
        }
        var nextIsLeftDir = xAxis < 0f;
        var isMoving = !Mathf.Approximately(xAxis, 0f);
        var isInRunningKey = Input.GetKey(KeyCode.LeftShift);
        var nextIsRunning = isInRunningKey && Stamina > 0f && isMoving;

        if(_isRunning != nextIsRunning)
        {
            _footStepAudioTimer = 0f;
        }
        _isRunning = nextIsRunning;

        if (isMoving)
        {
            // moving
            if(_isLeftDir != nextIsLeftDir && !_isShifting)
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
            Stamina += Time.deltaTime / 2f;
        }


        transform.Translate(velX * Time.deltaTime * Vector2.right);

        _animator.SetBool("IsRunning", _isRunning && !_isShifting);
        _animator.SetBool("IsWalking", isMoving && !_isShifting);
    }


    private void LightUpdate()
    {
        if(Input.GetKeyDown(KeyCode.W))
        {
            _isLighting = !_isLighting;
        }
        _handLight.gameObject.SetActive(_isLighting && LightEnerge > 0f && !_isShifting);

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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        _animator.SetBool("IsJumping", _isJumping);
        _animator.SetBool("IsFalling", _rigid.velocity.y < -0.01f);
    }

    public void Jump()
    {
        if(IsOnGround && _jumpCount > 0)
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

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (_steppingGrounds.Contains(collision.collider))
        {
            _steppingGrounds.Remove(collision.collider);
        }
    }
}
