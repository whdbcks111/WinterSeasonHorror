using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class HandLight : MonoBehaviour
{
    [SerializeField] private Light2D _light;
    [SerializeField] private float _minIntensity, _maxIntensity;

    private void Update()
    {
        _light.intensity = Random.Range(_minIntensity, _maxIntensity);
    }
}
