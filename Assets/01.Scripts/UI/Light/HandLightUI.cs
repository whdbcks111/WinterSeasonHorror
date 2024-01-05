using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandLightUI : MonoBehaviour
{
    public Image LightGuageFill, LightToggleIcon;
    public Sprite LightOnIcon, LightOffIcon;

    private void Update()
    {
        LightToggleIcon.sprite = Player.Instance.IsLightOn ? LightOnIcon : LightOffIcon;
        LightGuageFill.fillAmount = Player.Instance.LightEnerge / Player.Instance.MaxLightEnerge;
    }
}
