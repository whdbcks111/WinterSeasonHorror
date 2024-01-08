using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    [SerializeField] private Collider2D _feetCollider;
    [SerializeField] private Light2D _handLight, _surroundLight;

    [SerializeField] private Vector2 _camOffset;

    [SerializeField] private Color _hiddenColor;
    [SerializeField] private float _moveSpeed = 5;
    [SerializeField] private float _runModifier = 1.3f;
    [SerializeField] private float _jumpForce = 10;
    [SerializeField] private int _maxJumpCount = 1;
    
    public float MaxLightEnerge = 100;
    [HideInInspector] public float LightEnerge;

    public float MaxStamina = 20f;
    [HideInInspector] public float Stamina;

    private Rigidbody2D _rigid;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    private readonly HashSet<Collider2D> _steppingGrounds = new();
    private bool _isLeftDir = false;

    private int _jumpCount = 0;
    private bool _isJumping = false;
    private bool _isLeftJump = false;

    private float _handLightDefaultAngleX;
    private float _handLightOffsetX, _surroundLightOffsetX;

    private bool _isHidden = false;

    public bool IsHidden { get => _isHidden; }
    public bool IsLightOn { get => _handLight.gameObject.activeSelf; }
    public bool IsOnGround { get => _steppingGrounds.Count > 0; }
    public float MoveSpeed { get => _moveSpeed; }

    private void Awake()
    {
        Instance = this;
        LightEnerge = MaxLightEnerge;
        Stamina = MaxStamina;
        _handLightDefaultAngleX = _handLight.transform.eulerAngles.z;
        _rigid = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

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
        HiddenViewUpdate();
    }

    public void Hide() => _isHidden = true;
    public void Reveal() => _isHidden = false;

    private void HiddenViewUpdate()
    {
        _spriteRenderer.color = IsHidden ? _hiddenColor : Color.white;

    }

    private void MoveUpdate()
    {
        var isRunning = Input.GetKey(KeyCode.LeftShift) && Stamina > 0f;

        var xAxis = Input.GetAxisRaw("Horizontal");
        if(_isJumping && (xAxis > 0f && _isLeftJump || xAxis < 0f && !_isLeftJump))
        {
            xAxis = 0f;
        }

        if (!Mathf.Approximately(xAxis, 0f)) _isLeftDir = xAxis < 0f;
        var velX = xAxis * _moveSpeed;

        if(isRunning)
        {
            velX *= _runModifier;
            Stamina -= Time.deltaTime;
        }

        _animator.SetBool("IsRunning", isRunning);
        _animator.SetFloat("VelX", velX);
        _animator.SetBool("IsOnGround", IsOnGround);
        _animator.SetFloat("VelY", _rigid.velocity.y);

        transform.Translate(velX * Time.deltaTime * Vector2.right);

        _spriteRenderer.flipX = _isLeftDir;
        CameraController.Instance.SetOffset(_camOffset * (_isLeftDir ? -1 : 1), 0.7f);
    }

    private void LightUpdate()
    {
        if(Input.GetKeyDown(KeyCode.W))
        {
            _handLight.gameObject.SetActive(!_handLight.gameObject.activeSelf);
        }
        if(LightEnerge <= 0f) _handLight.gameObject.SetActive(false);

        if (IsLightOn) LightEnerge -= Time.deltaTime;

        int dir = _isLeftDir ? -1 : 1;
        _handLight.transform.localPosition = new(dir * _handLightOffsetX, _handLight.transform.localPosition.y);
        _surroundLight.transform.localPosition = new(dir * _surroundLightOffsetX, _surroundLight.transform.localPosition.y);
        _handLight.transform.eulerAngles = new(0, 0, dir * _handLightDefaultAngleX);
    }

    private void JumpUpdate()
    {
        if (IsOnGround && _rigid.velocity.y < 0.01f) _isJumping = false;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    public void Jump()
    {
        if(IsOnGround && _jumpCount > 0)
        {
            _isJumping = true;
            _jumpCount--;
            _animator.SetTrigger("Jump");
            _isLeftJump = _isLeftDir;
            _rigid.velocity = new(_rigid.velocity.x, _jumpForce);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (_feetCollider.bounds.min.y >= collision.GetContact(0).point.y &&
            _rigid.velocity.y <= 0.01f && !_steppingGrounds.Contains(collision.collider))
        {
            _steppingGrounds.Add(collision.collider);
            _jumpCount = _maxJumpCount;
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
