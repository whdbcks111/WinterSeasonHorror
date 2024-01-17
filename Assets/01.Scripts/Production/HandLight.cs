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
    private List<GameObject> objList = new List<GameObject>();
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            foreach (var o in objList)
            {
                o.SetActive(true);
                //objList.Remove(o);
            }
        }

        _light.intensity = Random.Range(_minIntensity, _maxIntensity);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if(other.transform.gameObject.layer == LayerMask.NameToLayer("Wall") ) return;
        objList.Add(other.transform.gameObject);
        other.transform.gameObject.SetActive(false);
    }
}
