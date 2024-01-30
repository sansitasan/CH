using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Player1DAnim : PlayerAnim
{
    public Player1DAnim(Animator tabiAnim, Animator bDAnim, SpriteRenderer tabiSprite) : base(tabiAnim, bDAnim, tabiSprite) { }

    public override void ChangeAnim(PlayerStates state)
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

            default:
                _tabiAnim.Play(state.ToString());
                break;
        }
    }
}
