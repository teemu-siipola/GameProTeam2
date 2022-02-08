using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigSounds : MonoBehaviour
{
    public SoundEffects sfx;

    AudioSource _source;
    float _timer;

    void Start()
    {
        SetTimer();
        _source = GetComponent<AudioSource>();
    }

    void Update()
    {
        _timer -= Time.deltaTime;
        if (_timer < 0)
        {
            SetTimer();
            _source.PlayOneShot(sfx.RandomPigSound());
        }
    }

    void SetTimer()
    {
        _timer = Random.Range(5f, 30f);
    }
}
