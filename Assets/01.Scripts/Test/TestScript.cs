using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var a = GetComponent<Image>();
        Debug.Log(a.sprite.name);
        // Update is called once per frame
    }

    void Update()
    {
        
    }
}
