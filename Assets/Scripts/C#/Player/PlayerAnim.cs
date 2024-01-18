using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerAnim : IDisposable
{
    private Animator _tabiAnim;
    private Animator _BDAnim;
    private SpriteRenderer _tabiSprite;
    private CancellationTokenSource _cts = new CancellationTokenSource();

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
        _tabiSprite.color = new Color(1, 1, 1, 0);
    }

    public async UniTask StartFadeAsync(float time = 1f)
    {
        _tabiSprite.color = new Color(1, 1, 1, 0);
        float dt = 0f;

        while (time > dt)
        {
            dt += Time.unscaledDeltaTime / time;
            _tabiSprite.color = Color.Lerp(new Color(1, 1, 1, 0), Color.white, dt);
            await UniTask.DelayFrame(1, cancellationToken: _cts.Token);
        }
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

            default:
                _tabiAnim.Play(state.ToString());
                break;
        }
    }

    public void Dispose()
    {
        _tabiAnim = null;
        _BDAnim = null;
        _cts.Cancel();
        _cts.Dispose();
    }
}
