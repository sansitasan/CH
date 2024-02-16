using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Character1DAnim : CharacterAnim
{
    private Rigidbody2D _rb;
    private Animator _BDAnim;
    private SpriteRenderer _BDSprite;
    private Transform _BDTransform;
    private readonly Vector3 _BDPosition;

    public Character1DAnim(GameObject tabi, GameObject BD, Rigidbody2D rb) : base(tabi) 
    {
        _rb = rb;
        _BDAnim = BD.GetComponent<Animator>();
        _BDSprite = BD.GetComponent<SpriteRenderer>();
        _BDTransform = BD.GetComponent<Transform>();
        _BDPosition = BD.transform.localPosition;
    }

    public override async UniTask StartFadeAsync(float time = 1f)
    {
        Color baseCol = new Color(1, 1, 1, 0);
        Color col;
        _sprite.color = baseCol;
        _BDSprite.color = baseCol;
        float dt = 0f;

        while (time > dt)
        {
            dt += Time.unscaledDeltaTime / time;
            col = Color.Lerp(baseCol, Color.white, dt);
            _sprite.color = col;
            _BDSprite.color = col;
            await UniTask.DelayFrame(1, cancellationToken: _cts.Token);
        }
    }

    public override async UniTask UseSkill(float time)
    {
        Color baseCol = new Color(1, 1, 1, 0);
        _bCoolTime = true;
        _BDSprite.color = baseCol;
        float dt = 0f;
        --time;
        while (time > dt)
        {
            dt += Time.deltaTime;
            await UniTask.DelayFrame(1, cancellationToken: _cts.Token);
        }

        dt = 0;

        while (1f > dt)
        {
            dt += Time.deltaTime;
            _BDSprite.color = Color.Lerp(baseCol, Color.white, dt);
            await UniTask.DelayFrame(1, cancellationToken: _cts.Token);
        }
        _bCoolTime = false;
    }

    public override void ChangeDir(Vector2 dir)
    {
        if (dir.x > 0)
        {
            _sprite.flipX = false;
            _BDSprite.flipX = false;
            _BDTransform.localPosition = _BDPosition;
        }

        else if (dir.x < 0)
        {
            _sprite.flipX = true;
            _BDSprite.flipX = true;
            _BDTransform.localPosition = -_BDPosition;
        }
    }

    public override void ChangeAnim(PlayerStates state)
    {
        switch (state)
        {
            case PlayerStates.Idle:
                _anim.SetBool("Move", false);
                _BDAnim.SetBool("Move", false);
                break;
            case PlayerStates.Move:
                _anim.SetBool("Move", true);
                _BDAnim.SetBool("Move", true);
                break;
            case PlayerStates.Jump:
                _anim.Play(state.ToString(), 0, 0);
                _BDAnim.Play(state.ToString());
                if (!_anim.GetBool("Jump"))
                {
                    _anim.SetBool("Jump", true);
                    if (_rb.velocity.y > 0)
                        _BDAnim.SetBool("Jump", true);
                }
                break;
            case PlayerStates.Landing:
                _anim.SetBool("Jump", false);
                _BDAnim.SetBool("Jump", false);
                break;

            case PlayerStates.Behave:
                _anim.Play(state.ToString());
                _BDAnim.SetBool("Move", false);
                break;

            default:
                _anim.Play(state.ToString());
                break;
        }
    }
}
