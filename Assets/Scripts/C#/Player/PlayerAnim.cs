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
    protected SpriteRenderer _BDSprite;
    protected Transform _BDTransform;
    protected readonly Vector3 _BDOriginPos;
    protected readonly Vector3 _BDFlipPos;
    protected CancellationTokenSource _cts = new CancellationTokenSource();
    protected bool _bCoolTime;
    public bool BCoolTime { get => _bCoolTime; }
    public Vector2 LookDir { get; protected set; }

    public short Flip { 
        get{
            if (_tabiSprite.flipX) return -1; 
            else return 1;
        }
    }

    public PlayerAnim(GameObject tabi, GameObject BD)
    {
        _tabiAnim = tabi.GetComponent<Animator>();
        _BDAnim = BD.GetComponent<Animator>();
        _tabiSprite = tabi.GetComponent<SpriteRenderer>();
        _BDSprite = BD.GetComponent<SpriteRenderer>();
        _BDTransform = BD.GetComponent<Transform>();
        _BDOriginPos = _BDTransform.localPosition;
        _BDFlipPos = new Vector3(-_BDOriginPos.x, _BDOriginPos.y, _BDOriginPos.z);
    }

    public async UniTask StartFadeAsync(float time = 1f)
    {
        _tabiSprite.color = new Color(1, 1, 1, 0);
        _BDSprite.color = new Color(1, 1, 1, 0);
        float dt = 0f;
        Color col;

        while (time > dt)
        {
            dt += Time.unscaledDeltaTime / time;
            col = Color.Lerp(new Color(1, 1, 1, 0), Color.white, dt);
            _tabiSprite.color = col;
            _BDSprite.color = col;
            await UniTask.DelayFrame(1, cancellationToken: _cts.Token);
        }
    }

    public async UniTask UseSkill(float time)
    {
        _bCoolTime = true;
        _BDSprite.color = new Color(1, 1, 1, 0);
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
            _BDSprite.color = Color.Lerp(new Color(1, 1, 1, 0), Color.white, dt);
            await UniTask.DelayFrame(1, cancellationToken: _cts.Token);
        }
        _bCoolTime = false;
    }

    public virtual void ChangeDir(Vector2 dir)
    {
        if (dir.x > 0)
        {
            _tabiSprite.flipX = false;
            _BDSprite.flipX = false;
            _BDTransform.localPosition = new Vector3(_BDOriginPos.x, _BDTransform.localPosition.y, 0);
        }
        else if (dir.x < 0)
        {
            _tabiSprite.flipX = true;
            _BDSprite.flipX = true;
            _BDTransform.localPosition = new Vector3(-_BDOriginPos.x, _BDTransform.localPosition.y, 0);
        }
    }

    public abstract void ChangeAnim(PlayerStates state);

    public void Dispose()
    {
        _tabiAnim = null;
        _BDAnim = null;
        _tabiSprite = null;
        _BDSprite = null;
        _cts.Cancel();
        _cts.Dispose();
    }
}
