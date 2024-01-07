using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingObject : MonoBehaviour
{
    public bool FollowX = true, FollowY = false;
    public float MaxSpeed, MinSpeed, MinDistance, MaxDistance;

    [HideInInspector] public bool IsFollowing = false;

    private void Update()
    {
        if(IsFollowing)
        {
            var distanceVector = Player.Instance.transform.position - transform.position;
            var dx = FollowX ? distanceVector.x : 0;
            var dy = FollowY ? distanceVector.y : 0;
            var distance = Mathf.Sqrt(Mathf.Pow(dx, 2) + Mathf.Pow(dy, 2));
            var dir = new Vector3(dx, dy).normalized;
 
            var speed = Mathf.Lerp(MinSpeed, MaxSpeed, (distance - MinDistance) / (MaxDistance - MinDistance));

            if (FollowX) transform.position += dir * speed * Time.deltaTime;
        }
    }
}
