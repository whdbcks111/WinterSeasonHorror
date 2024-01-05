using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeMoving : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _pupil;
    [SerializeField] private float _maxDistance;
    [SerializeField] private float _detectRange;
    [SerializeField] private float _smoothTime;

    private bool _isLighting = false;

    private Vector3 _vel = Vector3.zero;

    private void Update()
    {
        var playerDir = (Player.Instance.transform.position - transform.position);
        var targetPos = _isLighting ? 
            Vector3.zero :
            playerDir.normalized * 
                Mathf.Clamp(playerDir.magnitude / (_detectRange * _maxDistance), 0, _maxDistance);

        _pupil.transform.localPosition = Vector3.SmoothDamp(_pupil.transform.localPosition,
            targetPos,
            ref _vel, _smoothTime);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("HandLight"))
        {
            print("Light");
            _isLighting = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.CompareTag("HandLight"))
        {
            _isLighting = false;
        }
    }
}
