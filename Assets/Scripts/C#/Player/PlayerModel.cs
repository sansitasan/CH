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
    protected PlayerAnim _pa;
    [SerializeField]
    protected int _speed;

    protected List<IDisposable> _disposeList = new List<IDisposable>();

    void Awake()
    {
        Init();
        MakeBT();
    }

    protected virtual void Init()
    {
        _rb = GetComponent<Rigidbody2D>();
        _cts = new CancellationTokenSource();
        PlayerController playerController = new PlayerController(GetComponent<PlayerInput>().actions, this);
        _pa = new PlayerAnim(transform.GetChild(0).GetComponent<Animator>(), transform.GetChild(1).GetComponent<Animator>(), transform.GetChild(0).GetComponent<SpriteRenderer>());
        _disposeList.Add(playerController);
        _disposeList.Add(_pa);
    }

    protected abstract void MakeBT();

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
        _cts.Dispose();
        for (int i = 0; i < _disposeList.Count; ++i)
            _disposeList[i].Dispose();
    }
}
