using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public abstract class CharacterAnim : IDisposable
{
    protected Animator _anim;
    protected SpriteRenderer _sprite;
    protected Transform _transform;
    protected CancellationTokenSource _cts;
    protected bool _bCoolTime;
    public bool BCoolTime { get => _bCoolTime; }

    public short Flip
    {
        get
        {
            if (_sprite.flipX) return -1;
            else return 1;
        }
    }

    public CharacterAnim(GameObject go)
    {
        _anim = go.GetComponent<Animator>();
        _sprite = go.GetComponent<SpriteRenderer>();
        _transform = go.GetComponent<Transform>();
        _cts = new CancellationTokenSource();
    }

    public virtual async UniTask StartFadeAsync(float time = 1f)
    {
        _sprite.color = new Color(1, 1, 1, 0);
        float dt = 0f;
        Color col;

        while (time > dt)
        {
            dt += Time.unscaledDeltaTime / time;
            col = Color.Lerp(new Color(1, 1, 1, 0), Color.white, dt);
            _sprite.color = col;
            await UniTask.DelayFrame(1, cancellationToken: _cts.Token);
        }
    }

    public virtual async UniTask UseSkill(float time)
    {
        _bCoolTime = true;
        _sprite.color = new Color(1, 1, 1, 0);
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
            _sprite.color = Color.Lerp(new Color(1, 1, 1, 0), Color.white, dt);
            await UniTask.DelayFrame(1, cancellationToken: _cts.Token);
        }
        _bCoolTime = false;
    }

    public virtual void ChangeDir(Vector2 dir)
    {
        if (dir.x > 0)
            _sprite.flipX = false;

        else if (dir.x < 0)
            _sprite.flipX = true;
    }

    public abstract void ChangeAnim(PlayerStates state);

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
        _anim = null;
        _sprite = null;
    }
}
