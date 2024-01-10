using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using UnityEngine;


public class FootStepArea : MonoBehaviour
{
    public AudioClip FootStepClip;
    public float Volume = 1f, Pitch = 1f, RandomPitch = 0f;
    public float Delay = 0.1f, RandomDelay;
    public Vector3 SoundOffset;

    private string _key;
    private bool _alreadyIn = false;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_alreadyIn && collision.TryGetComponent(out Player p))
        {
            _alreadyIn = true;
            _key = "foot_step_" + UnityEngine.Random.Range(0, 1000);
            p.AddFootStepListener(_key, () =>
            {
                PlayDelayedFootStep().Forget();
            });
        }
    }

    private async UniTask PlayDelayedFootStep()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(Delay + UnityEngine.Random.Range(0f, RandomDelay)));

        SoundManager.Instance.PlaySFX(FootStepClip, Player.Instance.transform.position + SoundOffset, Volume,
            Pitch + UnityEngine.Random.Range(0f, RandomPitch), Player.Instance.transform);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_alreadyIn && collision.TryGetComponent(out Player p))
        {
            p.RemoveFootStepListener(_key); 
            _key = null;
            _alreadyIn = false;
        }
    }
}