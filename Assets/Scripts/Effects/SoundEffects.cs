using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="AudioClips")]
public class SoundEffects : ScriptableObject
{
    public AudioClip vacuumEmpty;
    public AudioClip vacuumSpit;
    public AudioClip vacuumSuck;
    public AudioClip vacuumEnd;
    public AudioClip vacuumLoop;
    public AudioClip vacuumStart;

    public AudioClip pigSnort1;
    public AudioClip pigSnort2;
    public AudioClip pigSnort3;
    public AudioClip pigSnort4;
    public AudioClip pigSuffer1;
    public AudioClip pigSuffer2;
    public AudioClip pigSucked1;
    public AudioClip pigSucked2;
    public AudioClip pigSucked3;

    public AudioClip gameWon;
    public AudioClip gameLost;

    public AudioClip RandomPigSucked()
    {
        int r = Random.Range(0, 3);
        switch (r)
        {
            case 0: return pigSucked1;
            case 1: return pigSucked2;
            default: return pigSucked3;
        }
    }
}
