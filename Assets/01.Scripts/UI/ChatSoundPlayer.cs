using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatSoundPlayer : MonoBehaviour
{
    public AudioSource Source;
    public float MaxVolume, Pitch, PlayTime = 0.1f;

    private float _timer = 0f;
    

    public void OnChat()
    {
        _timer = PlayTime;
    }

    private void Update()
    {
        if (_timer > 0f)
        {
            _timer -= Time.deltaTime;
        }

        Source.volume = _timer > 0f ? MaxVolume : 0f;
    }
}
