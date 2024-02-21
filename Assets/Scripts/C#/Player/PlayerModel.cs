using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public enum PlayerStates
{
    None = 0,
    Idle = 1,
    Move = 2,
    Behave = 3,
    BehaveCancel = 4,
    Skill = 5,
    Stage1 = 100,
    Jump = 101,
    InTheSky = 102,
    Landing = 103,
    Success = 99999
}

public abstract class PlayerModel : MonoBehaviour, IDisposable
{
    [SerializeField]
    protected Rigidbody2D _rb;

    protected CancellationTokenSource _cts;
    [SerializeField]
    protected BehaviourTree _tree;
    protected BlackBoard _blackBoard;
    protected PlayerController _controller;

    protected List<IDisposable> _disposeList = new List<IDisposable>();

    public virtual void Init(StageData so)
    {
        _rb = GetComponent<Rigidbody2D>();
        _cts = new CancellationTokenSource();
        _controller = new PlayerController(GetComponent<PlayerInput>().actions, this);
        _disposeList.Add(_cts);
        _disposeList.Add(_controller);
        DataInit(so);
        MakeBT(so);
    }

    public abstract void EditInit(StageData so);

    protected abstract void DataInit(StageData so);

    public abstract UniTask AfterScriptInit();

    public void Script(bool bStart)
    {
        if (bStart)
            _controller.DisableInput();
        else
            _controller.EnableInput();
    }

    protected abstract void MakeBT(StageData so);

    public virtual void PlayerInput(PlayerStates state)
    {
        _tree.CheckSeq(state);
    }

    public virtual void PlayerInput(PlayerStates state, Vector2 vector)
    {
        _blackBoard.MoveDir = vector;
        _tree.CheckSeq(state);
    }

    public void Dispose()
    {
        _cts.Cancel();
        for (int i = 0; i < _disposeList.Count; ++i)
            _disposeList[i].Dispose();
    }
}
