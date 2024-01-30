using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Player2DAnim : PlayerAnim
{
    public Player2DAnim(Animator tabiAnim, Animator bDAnim, SpriteRenderer tabiSprite) : base(tabiAnim, bDAnim, tabiSprite) { }

    public override void ChangeDir(Vector2 dir)
    {
        base.ChangeDir(dir);

        if (dir.y > 0)
            _tabiAnim.SetInteger("Y", 1);
        else if (dir.y < 0)
            _tabiAnim.SetInteger("Y", -1);
        else
            _tabiAnim.SetInteger("Y", 0);
    }

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
        }
    }
}
