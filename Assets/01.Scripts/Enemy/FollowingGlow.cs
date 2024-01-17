using Cysharp.Threading.Tasks.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingGlow : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject _target;

    private Vector3 _vel = Vector3.zero;
    [SerializeField] private float _smoothTime;

    // Update is called once per frame
    void Update()
    {
        if (_target != null)
        {
            gameObject.transform.position = Vector3.SmoothDamp(transform.position, _target.transform.position, ref _vel, _smoothTime);
        }
    }
}
