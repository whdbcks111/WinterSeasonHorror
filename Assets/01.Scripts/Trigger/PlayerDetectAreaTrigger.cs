using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerDetectAreaTrigger : DetectAreaTrigger
{
    protected override bool CheckCollision(Collider2D collider)
    {
        return collider.TryGetComponent(out Player _);
    }
}