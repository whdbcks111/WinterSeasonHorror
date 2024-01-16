using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatteryUI : MonoBehaviour
{
    [SerializeField] private float _showHoldTime = 2f, _showTime = 0.5f;  
    [SerializeField] private Image _frame, _filledImage;
    [SerializeField] private Color[] _sectionColors;
    [SerializeField] private float _min, _max;

    private void Update()
    {

        var progress = Player.Instance.LightEnerge / Player.Instance.MaxLightEnerge;
        _filledImage.fillAmount = progress * (_max - _min) + _min;
        _filledImage.color = SetRGB(_filledImage.color, 
            _sectionColors[Mathf.Clamp((int)((1 - progress) * _sectionColors.Length), 0, _sectionColors.Length - 1)]);

        var alphaAxis = Player.Instance.LightHoldingTime > _showHoldTime ? 1 : -1;
        var speed = alphaAxis / _showTime * Time.deltaTime;
        _frame.color = SetAlpha(_frame.color, _frame.color.a + speed);
        _filledImage.color = SetAlpha(_filledImage.color, _filledImage.color.a + speed);
    }

    private Color SetRGB(Color target, Color color)
    {
        return new Color(color.r, color.g, color.b, target.a);
    }

    private Color SetAlpha(Color target, float alpha)
    {
        return new Color(target.r, target.g, target.b, Mathf.Clamp01(alpha));
    }
}
