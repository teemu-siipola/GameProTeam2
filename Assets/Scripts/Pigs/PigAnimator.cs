using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigAnimator : MonoBehaviour
{
    private Animator _animator;
    private int _reset, _vacuum;
    void Awake()
    {
        _animator = GetComponent<Animator>();
        _reset = Animator.StringToHash("Idle");
        _vacuum = Animator.StringToHash("Vacuum");
    }

    public void ResetAnimations()
    {
        _animator.SetBool(_vacuum, false);
        //_animator.Play(_reset, 0);
    }

    public void Vacuum()
    {
        //_animator.SetTrigger(_vacuum);
        _animator.SetBool(_vacuum, true);
    }
}
