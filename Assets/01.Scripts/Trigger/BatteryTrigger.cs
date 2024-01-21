

using System;
using UnityEngine;

public class BatteryTrigger : BaseTrigger
{

    [Header("���͸� ���� ����")]
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