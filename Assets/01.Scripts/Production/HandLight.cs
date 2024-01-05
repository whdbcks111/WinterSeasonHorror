using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class HandLight : MonoBehaviour
{
    [SerializeField] private Light2D _light;
    [SerializeField] private float _minIntensity, _maxIntensity;

    private void Update()
    {
        _light.intensity = Random.Range(_minIntensity, _maxIntensity);
    }
}
