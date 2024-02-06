using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Player1DAnim : PlayerAnim
{
    public Player1DAnim(GameObject tabi, GameObject BD) : base(tabi, BD) { }

    public override void ChangeAnim(PlayerStates state)
    {
        switch (state)
        {
            case PlayerStates.Idle:
                _tabiAnim.SetBool("Move", false);
                _BDAnim.SetBool("Move", false);
                break;
            case PlayerStates.Move:
                _tabiAnim.SetBool("Move", true);
                _BDAnim.SetBool("Move", true);
                break;
            case PlayerStates.Jump:
                _tabiAnim.Play(state.ToString(), 0, 0);
                _BDAnim.Play(state.ToString());
                if (!_tabiAnim.GetBool("Jump"))
                {
                    _tabiAnim.SetBool("Jump", true);
                    _BDAnim.SetBool("Jump", true);
                }
                break;
            case PlayerStates.Landing:
                _tabiAnim.SetBool("Jump", false);
                _BDAnim.SetBool("Jump", false);
                break;

            case PlayerStates.Behave:
                _tabiAnim.Play(state.ToString());
                _BDAnim.SetBool("Move", false);
                break;

            default:
                _tabiAnim.Play(state.ToString());
                break;
        }
    }
}
