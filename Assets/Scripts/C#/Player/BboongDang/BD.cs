using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BD : MonoBehaviour
{
    private CharacterAnim _pa;
    private Rigidbody2D _rb;

    private Vector2 _curMoveDir;

    private List<IDisposable> _disposeList = new List<IDisposable>();

    public void Init(StageData data, Vector3 startPos)
    {
        _rb = GetComponent<Rigidbody2D>();
        _pa = new Character2DAnim(gameObject);
        transform.position = startPos;
    }

    public void SetPos(Vector3 position)
    {
        transform.position = position;
    }

    public void UpdateAnim(PlayerStates state)
    {
        _pa.ChangeAnim(state);
    }

    public void SetMoveData(Vector3 position, Vector2 moveDir)
    {
        _rb.MovePosition(position);

        if (_curMoveDir != moveDir)
            _pa.ChangeDir(moveDir);
        _curMoveDir = moveDir;
    }

    public void UseSkill(float time)
    {
        _pa.UseSkill(time).Forget();
    }

    public async UniTask AfterScriptInit()
    {
        await _pa.StartFadeAsync();
    }

    public void Dispose()
    {
        foreach (var dispose in _disposeList)
        {
            dispose.Dispose();
        }
    }
}
