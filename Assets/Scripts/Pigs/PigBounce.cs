using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigBounce : MonoBehaviour
{
    public SoundEffects sfx;

    AudioSource _source;

    void Awake()
    {
        _source = GetComponent<AudioSource>();
    }

    public void Bounce()
    {
        _source.PlayOneShot(sfx.pigBounce);
    }
}
