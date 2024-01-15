using UnityEngine;

[RequireComponent (typeof(Rigidbody2D))]
public class FallingObject : MonoBehaviour
{
    public AudioClip FallClip;
    public float VolumeMultiplier = 1f;


    private Rigidbody2D _rigid;
    private Vector2 _beforeVelocity;

    private void Awake()
    {
        _rigid = GetComponent<Rigidbody2D>();
        _beforeVelocity = _rigid.velocity;
    }

    private void Update()
    {
        if(_rigid.velocity.y >= 0f && _beforeVelocity.y < 0f)
        {
            SoundManager.Instance.PlaySFX(FallClip, transform.position, VolumeMultiplier * -_beforeVelocity.y, 1f);
        }
        _beforeVelocity = _rigid.velocity;
    }
}