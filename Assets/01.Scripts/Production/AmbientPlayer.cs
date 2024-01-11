using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientPlayer : MonoBehaviour
{
    [SerializeField] AudioClip[] _ambients;
    [SerializeField] private float _delay = 10f, _randomDelay = 10f;
    [SerializeField] private float _volume = 1f;

    private float _timer = 0f;
    public float Timer;

    private void SetDelay()
    {
        _timer = _delay + Random.Range(0f, _randomDelay);
    }

    private void Awake()
    {
        SetDelay();
    }

    private void Update()
    {
        Timer = _timer;
        if(_timer > 0f)
        {
            _timer -= Time.deltaTime;
        }
        else
        {
            SetDelay();
            SoundManager.Instance.PlaySFX(
                _ambients[Random.Range(0, _ambients.Length)],
                Player.Instance.transform.position, _volume, 1f, Player.Instance.transform);
        }
    }

}
