using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Player2DAnim : PlayerAnim
{
    public Player2DAnim(GameObject tabi, GameObject BD, PlayerModel model) : base(tabi, BD, model) 
    {

    }

    public override void ChangeDir(Vector2 dir)
    {
        base.ChangeDir(dir);

        if (dir.y > 0)
        {
            _tabiAnim.SetInteger("Y", 1);
            _BDAnim.SetInteger("Y", 1);
            LookDir = Vector2.up;
            _BDTransform.localPosition = new Vector3(_BDTransform.localPosition.x, -1, 0);
        }
        else if (dir.y < 0)
        {
            _tabiAnim.SetInteger("Y", -1);
            _BDAnim.SetInteger("Y", -1);
            LookDir = Vector2.down;
            _BDTransform.localPosition = new Vector3(_BDTransform.localPosition.x, 1, 0);
        }
        else
        {
            _tabiAnim.SetInteger("Y", 0);
            _BDAnim.SetInteger("Y", 0);
            if (_tabiSprite.flipX)
                LookDir = Vector2.left;
            else
                LookDir = Vector2.right;
        }
    }

    public override void ChangeAnim(PlayerStates state)
    {
        switch (state)
        {
            case PlayerStates.Idle:
                _tabiAnim.SetInteger("State", 0);
                _BDAnim.SetBool("Move", false);
                break;
            case PlayerStates.Move:
                _tabiAnim.SetInteger("State", 1);
                _BDAnim.SetBool("Move", true);
                break;
            case PlayerStates.Skill:
                _tabiAnim.SetInteger("State", 2);
                break;
            case PlayerStates.Dead:
                _tabiAnim.SetInteger("State", 3);
                _BDAnim.SetBool("Move", false);
                break;
        }
    }
}
