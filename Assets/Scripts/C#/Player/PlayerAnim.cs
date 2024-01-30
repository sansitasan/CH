using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public abstract class PlayerAnim : IDisposable
{
    protected Animator _tabiAnim;
    protected Animator _BDAnim;
    protected SpriteRenderer _tabiSprite;
    protected CancellationTokenSource _cts = new CancellationTokenSource();

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

    public virtual void ChangeDir(Vector2 dir)
    {
        if (dir.x > 0)
            _tabiSprite.flipX = false;
        else if (dir.x < 0)
            _tabiSprite.flipX = true;
    }

    public abstract void ChangeAnim(PlayerStates state);

    public void Dispose()
    {
        _tabiAnim = null;
        _BDAnim = null;
        _cts.Cancel();
        _cts.Dispose();
    }
}
