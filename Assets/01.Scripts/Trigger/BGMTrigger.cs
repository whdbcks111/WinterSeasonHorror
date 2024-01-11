

using System;
using UnityEngine;

public class BGMTrigger : BaseTrigger
{
    public AudioClip Clip;
    public bool Loop;
    public float Volume, Pitch, RandomPitch;

    public override void Enter()
    {
        SoundManager.Instance.PlayBGM(Clip, Loop, Volume, Pitch + UnityEngine.Random.Range(0, RandomPitch));
    }

    public override void Exit()
    {
    }
}