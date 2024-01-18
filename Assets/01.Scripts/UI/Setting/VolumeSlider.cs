using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class VolumeSlider : MonoBehaviour
{
    public Type SoundType;
    private Slider _slider;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    private void Start()
    {
        switch (SoundType)
        {
            case Type.BGM:
                _slider.value = SoundManager.Instance.GetBGMVolume();
                break;
            case Type.SFX:
                _slider.value = SoundManager.Instance.GetSFXVolume();
                break;
        }
    }

    public void SetVolume(float volume)
    {
        switch(SoundType)
        {
            case Type.BGM:
                SoundManager.Instance.SetBGMVolume(volume);
                break;
            case Type.SFX:
                SoundManager.Instance.SetSFXVolume(volume);
                break;
        }
    }

    public enum Type
    {
        BGM, SFX
    }
}
