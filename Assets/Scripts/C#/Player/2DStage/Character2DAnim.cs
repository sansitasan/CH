using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Character2DAnim : CharacterAnim
{
    public Vector2 LookDir { get; private set; }

    public Character2DAnim(GameObject go) : base(go) 
    {

    }

    public override void ChangeDir(Vector2 dir)
    {
        base.ChangeDir(dir);

        if (dir.y > 0.71f)
        {
            _anim.SetInteger("Y", 1);
            LookDir = Vector2.up;
        }
        else if (dir.y < -0.71f)
        {
            _anim.SetInteger("Y", -1);
            LookDir = Vector2.down;
        }
        else
        {
            _anim.SetInteger("Y", 0);
            if (_sprite.flipX)
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
                _anim.SetInteger("State", 0);
                break;
            case PlayerStates.Move:
                _anim.SetInteger("State", 1);
                break;
            case PlayerStates.Skill:
                _anim.SetInteger("State", 2);
                break;
            case PlayerStates.Dead:
                _anim.SetInteger("State", 3);
                break;
        }
    }
}
