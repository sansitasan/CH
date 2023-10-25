using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnim : IDisposable
{
    private Animator _tabiAnim;
    private Animator _BDAnim;
    public short Flip { get => _flip; }
    private short _flip;

    public PlayerAnim(Animator tabiAnim, Animator bDAnim)
    {
        _tabiAnim = tabiAnim;
        _BDAnim = bDAnim;
        _flip = 1;
    }

    public void ChangeDir(Vector2 dir)
    {
        if (dir.x > 0)
            _flip = 1;
        else if (dir.x < 0)
            _flip = -1;
    }

    public void AnimChange()
    {

    }

    public void Dispose()
    {
        _tabiAnim = null;
        _BDAnim = null;
    }
}
