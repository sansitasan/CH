using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnim : IDisposable
{
    private Animator _tabiAnim;
    private Animator _BDAnim;
    private SpriteRenderer _tabiSprite;
    public short Flip { 
        get{
            if (_tabiSprite.flipX) return -1; 
            else return 1;
        }
    }

    public PlayerAnim(Animator tabiAnim, Animator bDAnim, SpriteRenderer tabiSprite)
    {
        _tabiAnim = tabiAnim;
        _BDAnim = bDAnim;
        _tabiSprite = tabiSprite;
    }

    public void ChangeDir(Vector2 dir)
    {
        if (dir.x > 0)
            _tabiSprite.flipX = false;
        else if (dir.x < 0)
            _tabiSprite.flipX = true;
    }

    public void ChangeAnim(PlayerStates state)
    {
        switch (state)
        {
            case PlayerStates.Idle:
                _tabiAnim.SetBool("Move", false);
                break;
            case PlayerStates.Move:
                _tabiAnim.SetBool("Move", true);
                break;
            case PlayerStates.Jump:
                _tabiAnim.Play(state.ToString());
                _tabiAnim.SetBool("Jump", true);
                break;
            case PlayerStates.Landing:
                _tabiAnim.SetBool("Jump", false);
                break;
        }
    }

    public void Dispose()
    {
        _tabiAnim = null;
        _BDAnim = null;
    }
}
