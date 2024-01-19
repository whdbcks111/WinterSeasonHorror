using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VignettePingPong : MonoBehaviour
{
    public float Min, Max, PingPongTime;
    public VolumeProfile VolumeProfile;

    private void Update()
    {
        if(VolumeProfile.TryGet(out Vignette vignette))
        {
            vignette.intensity.Override(
                Mathf.Lerp(Min, Max, (Mathf.Sin(Time.time * Mathf.PI / PingPongTime) + 1f) / 2f)
                );
        }
    }
}
