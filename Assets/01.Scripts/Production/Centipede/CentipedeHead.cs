using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeHead : MonoBehaviour
{
    [SerializeField] private GameObject _bodyPrefab;
    [SerializeField] private GameObject _tailPrefab;

    [SerializeField] private int _bodyCount = 7;
    [SerializeField] private float _bodyDistance = 0.3f;

    private readonly List<GameObject> _children = new();
    private Vector3 _beforePos;

    private float _targetAngle;

    private void Awake()
    {
        for(int i = 0; i < _bodyCount; i++)
        {
            var body = Instantiate(_bodyPrefab, transform.position + _bodyDistance * -i * transform.up, 
                Quaternion.identity);
            _children.Add(body);
        }
        _children.Add(Instantiate(_tailPrefab, transform.position + _bodyDistance * -_bodyCount * transform.up,
            Quaternion.identity));
        _beforePos = transform.position;
        _targetAngle = transform.eulerAngles.z;
    }

    private void Update() 
    {
        var headDir = transform.position - _beforePos;
        if(headDir.sqrMagnitude > 0.3f * 0.3f)
        {
            _targetAngle = Mathf.Atan2(headDir.y, headDir.x) * Mathf.Rad2Deg - 90f;
            _beforePos = transform.position;
        }
        transform.eulerAngles = new(0, 0, Mathf.MoveTowardsAngle(transform.eulerAngles.z, _targetAngle, Time.deltaTime * 360f));

        var before = gameObject;
        foreach(var child in _children)
        {
            child.transform.up = (before.transform.position - child.transform.position).normalized;
            child.transform.position = before.transform.position + _bodyDistance * -child.transform.up;
            before = child;
        }
    }
}
