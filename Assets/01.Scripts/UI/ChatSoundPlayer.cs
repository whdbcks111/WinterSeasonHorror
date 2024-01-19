using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatSoundPlayer : MonoBehaviour
{
    public AudioClip Clip;
    public float Volume = 1f, Pitch = 1f, PlayTime = 0.1f, Delay = 0.2f;

    private float _timer = 0f, _delayTimer = 0f;
    

    public void OnChat(char ch)
    {
        if(ch != '.' && ch != ' ')
            _timer = PlayTime;
    }

    private void Update()
    {
        if (_timer > 0f)
        {
            _timer -= Time.deltaTime;

            if(_delayTimer > 0f) 
            { 
                _delayTimer -= Time.deltaTime;
            }
            else
            {
                _delayTimer += Delay;
                SoundManager.Instance.PlaySFX(Clip, Player.Instance.transform.position, Volume, Pitch);
            }
        }
        else
        {
            _delayTimer = 0f;
        }
    }
}
