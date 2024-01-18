using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class LightDetectAreaTrigger : DetectAreaTrigger
{
    public bool ShowTriggerBounds = true;

    private void OnDrawGizmos()
    {
        if (!ShowTriggerBounds) return;
        Gizmos.color = Color.yellow;
        var colliders = GetComponentsInChildren<Collider2D>();
        foreach (var collider in colliders)
        {
            Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
        }
    }

    protected override bool CheckCollision(Collider2D collider)
    {
        return collider.CompareTag("HandLight");
    }
}