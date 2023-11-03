using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WallBlink : MonoBehaviour, IDisposable
{
    [SerializeField]
    private float _blinkTime;
    [SerializeField]
    private float _lightOnTime;
    private CancellationTokenSource _cts = new CancellationTokenSource();
    private Tilemap _wall;
    private Color _baseColor;

    private void Awake()
    {
        _wall = GetComponent<Tilemap>();
        _baseColor = new Color(1, 1, 1, 0);
    }

    public async UniTask BlinkAsync()
    {
        float time = 0;

        while (time < _blinkTime)
        {
            time += Time.deltaTime;
            _wall.color = Color.Lerp(_baseColor, Color.white, time / _blinkTime);
            await UniTask.DelayFrame(1, cancellationToken: _cts.Token);
        }

        await UniTask.Delay(TimeSpan.FromSeconds(_lightOnTime), cancellationToken: _cts.Token);

        while (time > 0)
        {
            time -= Time.deltaTime;
            _wall.color = Color.Lerp(_baseColor, Color.white, time / _blinkTime);
            await UniTask.DelayFrame(1, cancellationToken: _cts.Token);
        }
    }

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
    }
}
