

using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Platform : MonoBehaviour
{
    private Collider2D[] _colliders;

    private void Awake()
    {
        _colliders = GetComponentsInChildren<Collider2D>();
    }

    private void Update()
    {
        foreach (var collider in _colliders)
        {
            collider.isTrigger = !(collider.bounds.max.y <= Player.Instance.FeetPositionY && Player.Instance.Velocity.y <= 0f);
        }
    }
}