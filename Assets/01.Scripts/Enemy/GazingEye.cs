using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazingEye : MonoBehaviour
{
    private GameObject _pupil;
    private GameObject _eyeSphere;
    private float _eyeMaxDistance = 1.5f;
    [SerializeField] private float _smoothTime;
    private Vector3 _velocity = Vector3.zero;

    private float _scaleValue;

    // Start is called before the first frame update

    
    void Start()
    {
        _eyeSphere = gameObject;
        _pupil = transform.GetChild(0).gameObject;

        _scaleValue = transform.localScale.x;

    }

    // Update is called once per frame
    void Update()
    {
        GazingPlayer();
    }
    void GazingPlayer()
    {
        var Direction = (Player.Instance.transform.position - _eyeSphere.transform.position);
        var PupilPos = Direction.normalized * Mathf.Clamp(Direction.magnitude, 0, _eyeMaxDistance);
        _pupil.transform.localPosition = Vector3.SmoothDamp(_pupil.transform.localPosition, PupilPos, ref _velocity, _smoothTime);
    }
}
