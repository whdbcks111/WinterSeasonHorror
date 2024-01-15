using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class LightDetectAreaTrigger : DetectAreaTrigger
{

    protected override bool CheckCollision(Collider2D collider)
    {
        return collider.CompareTag("HandLight");
    }
}