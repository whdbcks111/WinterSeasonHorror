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

    [SerializeField] private float _moveSpeed = 5;
    [SerializeField] private float _jumpForce = 10;
    [SerializeField] private int _maxJumpCount = 1;
    public float MaxLightEnerge = 100;
    [HideInInspector] public float LightEnerge;

    private Rigidbody2D _rigid;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private int _jumpCount = 0;
    private readonly HashSet<Collider2D> _steppingGrounds = new();
    private bool _isLeftDir = false;
    private float _handLightDefaultAngleX;
    private float _handLightOffsetX, _surroundLightOffsetX;

    public bool IsLightOn { get => _handLight.gameObject.activeSelf; }
    public bool IsOnGround { get => _steppingGrounds.Count > 0; }
    public float MoveSpeed { get => _moveSpeed; }

    private void Awake()
    {
        Instance = this;
        LightEnerge = MaxLightEnerge;
        _handLightDefaultAngleX = _handLight.transform.eulerAngles.z;
        _rigid = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _handLightOffsetX = _handLight.transform.position.x;
        _surroundLightOffsetX = _surroundLight.transform.position.x;
    }

    private void Start()
    {
        CameraController.Instance.SetFocus(transform, 0.5f);
    }

    private void Update()
    {
        JumpUpdate();
        LightUpdate();
    }

    private void FixedUpdate()
    {
        MoveUpdate();
    }

    private void MoveUpdate()
    {
        var xAxis = Input.GetAxisRaw("Horizontal");
        if (!Mathf.Approximately(xAxis, 0f)) _isLeftDir = xAxis < 0f;
        var velX = xAxis * _moveSpeed;

        _animator.SetFloat("VelX", velX);
        _animator.SetBool("IsOnGround", IsOnGround);
        _animator.SetFloat("VelY", _rigid.velocity.y);

        var befPos = _rigid.position;
        _rigid.MovePosition(_rigid.position + velX * Time.fixedDeltaTime * Vector2.right);
        Physics2D.SyncTransforms();
        print($"{befPos} {_rigid.position}");

        _spriteRenderer.flipX = _isLeftDir;
        CameraController.Instance.SetOffset(_camOffset * (_isLeftDir ? -1 : 1), 0.7f);
    }

    private void LightUpdate()
    {

        if(Input.GetKeyDown(KeyCode.W))
        {
            _handLight.gameObject.SetActive(!_handLight.gameObject.activeSelf);
        }

        if (IsLightOn) LightEnerge -= Time.deltaTime;

        int dir = _isLeftDir ? -1 : 1;
        _handLight.transform.localPosition = new(dir * _handLightOffsetX, _handLight.transform.localPosition.y);
        _surroundLight.transform.localPosition = new(dir * _surroundLightOffsetX, _surroundLight.transform.localPosition.y);
        _handLight.transform.eulerAngles = new(0, 0, dir * _handLightDefaultAngleX);
    }

    private void JumpUpdate()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    public void Jump()
    {
        _animator.SetTrigger("Jump");
        if(IsOnGround && _jumpCount > 0)
        {
            _jumpCount--;
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
