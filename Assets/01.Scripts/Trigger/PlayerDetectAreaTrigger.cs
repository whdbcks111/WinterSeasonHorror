using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerDetectAreaTrigger : DetectAreaTrigger
{
    public bool ShowTriggerBounds = true;

    private void OnDrawGizmos()
    {
        if(!ShowTriggerBounds) return;
        var colliders = GetComponentsInChildren<Collider2D>();
        Gizmos.color = new Color(0f, 1f, 1f);
        foreach (var collider in colliders)
        {
            Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
        }
    }

    protected override bool CheckCollision(Collider2D collider)
    {
        return collider.TryGetComponent(out Player _);
    }
}