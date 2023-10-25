using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerController : IDisposable
{
    private List<InputAction> _actions;
    private PlayerModel _model;

    private bool _bMove;
    private Vector2 pos;
    private CancellationTokenSource _moveCts = new CancellationTokenSource();

    public PlayerController(InputActionAsset iaa, PlayerModel model)
    {
        _model = model;
        _actions = new List<InputAction>(3) { iaa.FindAction("Move"), iaa.FindAction("Behave"), iaa.FindAction("Skill") };
        _actions[0].performed += Move;
        _actions[0].canceled += Idle;
        _actions[1].performed += Behave;
        _actions[1].canceled += BehaveCancel;
        _actions[2].performed += Skill;
    }

    private void Idle(CallbackContext ctx)
    {
        _model.PlayerInput(PlayerStates.Idle);
        _bMove = false;
    }

    private void Move(CallbackContext ctx)
    {
        pos = ctx.ReadValue<Vector2>();
        if (!_bMove)
        {
            _bMove = true;
            MoveInputAsync().Forget();
        }
    }

    private async UniTaskVoid MoveInputAsync()
    {
        while (_bMove)
        {
            _model.PlayerInput(PlayerStates.Move, pos);
            await UniTask.DelayFrame(1, cancellationToken: _moveCts.Token);
        }
    }

    private void Behave(CallbackContext ctx)
    {
        _model.PlayerInput(PlayerStates.Behave);
    }

    private void BehaveCancel(CallbackContext ctx)
    {
        _model.PlayerInput(PlayerStates.BehaveCancel);
    }

    private void Skill(CallbackContext ctx)
    {
        _model.PlayerInput(PlayerStates.Skill);
    }

    public void Dispose()
    {
        _moveCts.Cancel();
        _moveCts.Dispose();
        _actions[0].performed -= Move;
        _actions[0].canceled -= Idle;
        _actions[1].performed -= Behave;
        _actions[1].canceled -= BehaveCancel;
        _actions[2].performed -= Skill;
        _actions.Clear();
        _actions = null;
        _model = null;
    }
}
