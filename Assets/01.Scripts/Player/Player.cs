using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    [SerializeField] private Collider2D _feetCollider;
    [SerializeField] private Light2D _handLight;

    [SerializeField] private Vector2 _camOffset;
    [SerializeField] private float _lightOffsetX, _lightMinAngle, _lightMaxAngle;

    [SerializeField] private float _moveSpeed = 5;
    [SerializeField] private float _jumpForce = 10;
    [SerializeField] private int _maxJumpCount = 1;

    private Rigidbody2D _rigid;
    private int _jumpCount = 0;
    private readonly HashSet<Collider2D> _steppingGrounds = new();
    private bool _isLeftDir = false;

    public bool IsOnGround { get => _steppingGrounds.Count > 0; }

    private void Awake()
    {
        Instance = this;
        _rigid = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        CameraController.Instance.SetFocus(transform, 0.5f);
    }

    private void Update()
    {
        MoveUpdate();
        JumpUpdate();
        HandLightUpdate();
    }

    private void MoveUpdate()
    {
        var xAxis = Input.GetAxisRaw("Horizontal");
        if (!Mathf.Approximately(xAxis, 0f)) _isLeftDir = xAxis < 0f;
        transform.Translate(xAxis * _moveSpeed * Time.deltaTime * Vector3.right);

        CameraController.Instance.SetOffset(_camOffset * (_isLeftDir ? -1 : 1), 0.7f);
    }

    private void HandLightUpdate()
    {
        int dir = _isLeftDir ? -1 : 1;
        _handLight.transform.localPosition = new(dir * _lightOffsetX, _handLight.transform.localPosition.y);
        _handLight.transform.up = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)_handLight.transform.position;
        
        if(!ExtraMath.IsAngleBetween(_handLight.transform.eulerAngles.z, _lightMinAngle * dir, _lightMaxAngle * dir))
        {
            if (ExtraMath.GetAngleDistance(_handLight.transform.eulerAngles.z, _lightMinAngle * dir) 
                < ExtraMath.GetAngleDistance(_handLight.transform.eulerAngles.z, _lightMaxAngle * dir))
            {
                _handLight.transform.eulerAngles = new(0, 0, _lightMinAngle * dir);
            }
            else
            {
                _handLight.transform.eulerAngles = new(0, 0, _lightMaxAngle * dir);
            }
        }
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
