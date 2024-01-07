using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTrigger : MonoBehaviour
{
    public FollowingObject FollowingObject;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!FollowingObject.IsFollowing && collision.TryGetComponent(out Player _))
        {
            FollowingObject.IsFollowing = true;
        }
    }
}
