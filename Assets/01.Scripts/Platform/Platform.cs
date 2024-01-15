

using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer))]
public class Platform : MonoBehaviour
{
    private BoxCollider2D[] _colliders = null;
    private SpriteRenderer _renderer = null;

    private void Update()
    {
        if(_renderer == null) _renderer = GetComponent<SpriteRenderer>();
        if (_colliders == null) _colliders = GetComponentsInChildren<BoxCollider2D>();

        foreach (var collider in _colliders)
        {
            if(Player.Instance != null)
                collider.isTrigger = !(collider.bounds.max.y <= Player.Instance.FeetPositionY && Player.Instance.Velocity.y <= 0f);
            collider.size = new(_renderer.size.x, collider.size.y);
        }
    }
}