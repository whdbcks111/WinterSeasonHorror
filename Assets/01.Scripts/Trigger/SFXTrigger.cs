

using System;
using UnityEngine;

public class SFXTrigger : BaseTrigger
{
    public AudioClip Clip;
    public float Volume, Pitch, RandomPitch;

    public override void Enter()
    {
        SoundManager.Instance.PlaySFX(Clip, transform.position, Volume, Pitch, transform);
    }

    public override void Exit()
    {
    }
}