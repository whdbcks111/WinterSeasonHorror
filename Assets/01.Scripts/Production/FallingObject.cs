using UnityEngine;

public class FallingObject : MonoBehaviour
{
    public AudioClip FallClip;
    public float VolumeMultiplier = 1f;


    private Vector3 _beforePos;
    private Vector3 _velocity, _beforeVelocity;

    private void Awake()
    {
    }

    private void FixedUpdate()
    {
        _beforeVelocity = _velocity;
        _velocity = (transform.position - _beforePos) / Time.fixedDeltaTime;
        _beforePos = transform.position;

        if(_velocity.y >= 0f && _beforeVelocity.y < 0f)
        {
            SoundManager.Instance.PlaySFX(FallClip, transform.position, VolumeMultiplier * -_beforeVelocity.y, 1f);
        }
    }
}