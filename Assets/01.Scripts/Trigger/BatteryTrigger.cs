

using System;
using UnityEngine;

public class BatteryTrigger : BaseTrigger
{

    [Header("배터리 충전 사운드")]
    [SerializeField] private AudioClip _clip;
    [SerializeField] private float _volume;

    public override void Enter()
    {
        Player.Instance.LightEnerge = Player.Instance.MaxLightEnerge;
        SoundManager.Instance.PlaySFX(_clip, transform.position, _volume);
    }

    public override void Exit()
    {
    }
}