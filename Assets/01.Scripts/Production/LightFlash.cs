using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class LightFlash : MonoBehaviour
{
    [SerializeField] private float _time;
    [SerializeField] private float _minIntensity, _maxIntensity;

    private Light2D _light;

    private void Awake()
    {
        _light = GetComponent<Light2D>();
    }

    private void Update()
    {
        _light.intensity = Mathf.Lerp(_minIntensity, _maxIntensity, (Mathf.Sin(Time.time / _time * Mathf.PI) + 1f) / 2f);
    }

}
