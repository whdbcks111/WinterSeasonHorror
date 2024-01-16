using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandLightUI : MonoBehaviour
{
    public Image LightToggleIcon;
    public Sprite LightOnIcon, LightOffIcon;

    private void Update()
    {
        LightToggleIcon.sprite = Player.Instance.IsLightOn ? LightOnIcon : LightOffIcon;
    }
}
