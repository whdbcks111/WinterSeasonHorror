using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatteryUI : MonoBehaviour
{
    [SerializeField] private Image _filledImage;
    [SerializeField] private Color[] _sectionColors;
    [SerializeField] private float _min, _max;

    private void Update()
    {
        var progress = Player.Instance.LightEnerge / Player.Instance.MaxLightEnerge;
        _filledImage.fillAmount = progress * (_max - _min) + _min;
        _filledImage.color = _sectionColors[Mathf.Clamp((int)((1 - progress) * _sectionColors.Length), 0, _sectionColors.Length - 1)];
    }
}
