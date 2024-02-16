using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Character2DAnim : CharacterAnim
{
    public Character2DAnim(GameObject go) : base(go) 
    {

    }

    public override void ChangeDir(Vector2 dir)
    {
        base.ChangeDir(dir);

        if (dir.y > 0.71f)
        {
            _anim.SetInteger("Y", 1);
            //_model.LookDir = Vector2.up;
        }
        else if (dir.y < -0.71f)
        {
            _anim.SetInteger("Y", -1);
            //_model.LookDir = Vector2.down;
        }
        else
        {
            _anim.SetInteger("Y", 0);
            //if (_sprite.flipX)
            //    _model.LookDir = Vector2.left;
            //else
            //    _model.LookDir = Vector2.right;
        }
    }

    public override void ChangeAnim(PlayerStates state)
    {
        switch (state)
        {
            case PlayerStates.Idle:
                _anim.SetBool("Move", false);
                break;
            case PlayerStates.Move:
                _anim.SetBool("Move", true);
                break;
        }
    }
}
