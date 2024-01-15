using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using UnityEngine;


public class FootStepTrigger : BaseTrigger
{
    [Header("발자국 사운드 클립")]
    public AudioClip FootStepClip;
    public float Volume = 1f, Pitch = 1f, RandomPitch = 0f;
    public float Delay = 0.1f, RandomDelay;
    public Vector3 SoundOffset;

    private string _key;

    private async UniTask PlayDelayedFootStep()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(Delay + UnityEngine.Random.Range(0f, RandomDelay)));

        SoundManager.Instance.PlaySFX(FootStepClip, Player.Instance.transform.position + SoundOffset, Volume,
            Pitch + UnityEngine.Random.Range(0f, RandomPitch), Player.Instance.transform);
    }

    public override void Enter()
    {
        _key = GetHashCode().ToString();
        Player.Instance.AddFootStepListener(_key, () =>
        {
            PlayDelayedFootStep().Forget();
        });
    }

    public override void Exit()
    {
        Player.Instance.RemoveFootStepListener(_key);
        _key = null;
    }
}