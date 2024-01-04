using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;

    private Rigidbody2D _rigid;

    private void Awake()
    {
        _rigid = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        MoveUpdate();
        JumpUpdate();
    }

    private void MoveUpdate()
    {
        var xAxis = Input.GetAxisRaw("Horizontal");
        transform.Translate(xAxis * Time.deltaTime * Vector3.right);
    }

    private void JumpUpdate()
    {

    }
}
