using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class IdleLeaf : BehaviourLeaf
{
    public IdleLeaf(BlackBoard board) : base(board, PlayerStates.Idle) {  }

    public override SeqStates CheckLeaf(PlayerStates ps)
    {
        Enter();
        return SeqStates.Running;
    }

    protected override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
    }
}

public abstract class MoveLeaf : BehaviourLeaf
{
    public MoveLeaf(BlackBoard board) : base(board, PlayerStates.Move) { }

    public override sealed SeqStates CheckLeaf(PlayerStates ps)
    {
        if (ps == PlayerStates.Move)
        {
            _seqStates = SeqStates.Running;
            Enter();
        }

        else if (ps == PlayerStates.Idle)
        {
            Exit();
            _seqStates = SeqStates.Fail;
        }

        return _seqStates;
    }

    protected override void Enter()
    {
        base.Enter();
        _blackBoard.PA.ChangeDir(_blackBoard.MoveDir);
    }

    public override void CancelBehaviour()
    {
        base.CancelBehaviour();
    }
}

public class Stage1MoveLeaf : MoveLeaf
{
    public Stage1MoveLeaf(BlackBoard board) : base(board) { }

    public override void Update()
    {
        _blackBoard.RD.velocity = new Vector3(_blackBoard.MoveDir.x, 0, 0).normalized 
            * _blackBoard.Data.Speed;
    }

    public override void Exit()
    {
        //_blackBoard.RD.velocity = Vector2.zero;
    }
}

public class Stage2MoveLeaf : MoveLeaf
{
    public Stage2MoveLeaf(BlackBoard board) : base(board) { }

    public override void Exit()
    {
        _blackBoard.RD.velocity = Vector2.zero;
    }

    public override void Update()
    {
        _blackBoard.RD.velocity = new Vector3(_blackBoard.MoveDir.x, _blackBoard.MoveDir.y, 0).normalized
            * _blackBoard.Data.Speed;
    }
}

public abstract class BehaveLeaf : BehaviourLeaf
{
    public BehaveLeaf(BlackBoard board, PlayerStates ps) : base(board, ps) { }

    public override SeqStates CheckLeaf(PlayerStates ps)
    {
        if (ps == PlayerStates.Behave && _seqStates == SeqStates.Fail)
        {
            Enter();
            _seqStates = SeqStates.Running;
        }

        else if (ps == PlayerStates.BehaveCancel)
        {
            Behave();
            _seqStates = SeqStates.Fail;
        }
        return _seqStates;
    }

    protected override void Enter()
    {
        base.Enter();
    }

    protected abstract void Behave();

    public override void CancelBehaviour()
    {
        base.CancelBehaviour();
    }
}

public class Stage1BehaveLeaf : BehaveLeaf
{
    private float _chargeTime;
    private Stage1BlackBoard _board;

    public Stage1BehaveLeaf(BlackBoard board) : base(board, PlayerStates.Behave)
    { 
        _board = board as Stage1BlackBoard;
    }

    protected override void Behave()
    {
        for (int i = 0; i < _board.Data.ChargePower.Length; ++i)
        {
            if (_chargeTime / _board.Data.ChargeTime <= i + 1)
            {
                if (_blackBoard.MoveDir == Vector2.zero)
                {
                    _blackBoard.RD.velocity = Vector2.up * _board.Data.ChargePower[i];
                }
                else
                    _blackBoard.RD.velocity = new Vector2(_blackBoard.PA.Flip * 0.5f, 0.866f).normalized * _board.Data.ChargePower[i];
                break;
            }
        }

        _chargeTime = 0;
        _blackBoard.Tree.CheckSeq(PlayerStates.InTheSky);
    }

    public override void Update()
    {
        if (_chargeTime + Time.fixedDeltaTime > 3)
            _chargeTime = 3;
        else
            _chargeTime += Time.fixedDeltaTime;
    }

    protected override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        _chargeTime = 0;
    }
}

public class SkyLeaf : BehaviourLeaf
{
    public SkyLeaf(BlackBoard board) : base(board, PlayerStates.Behave) { }

    public override SeqStates CheckLeaf(PlayerStates ps)
    {
        if (ps == PlayerStates.InTheSky && _seqStates == SeqStates.Fail)
        {
            _seqStates = SeqStates.Running;
            _blackBoard.PA.ChangeAnim(PlayerStates.Jump);
        }

        else if (_seqStates == SeqStates.Running && ps == PlayerStates.Landing)
        {
            Exit();
            _seqStates = SeqStates.Fail;
        }

        return _seqStates;
    }

    protected override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        _blackBoard.PA.ChangeAnim(PlayerStates.Landing);
    }

    public override void Update()
    {

    }
}

public class SkillLeaf : BehaviourLeaf
{
    private Stage1BlackBoard _board;

    public SkillLeaf(BlackBoard board) : base(board, PlayerStates.Skill)
    {
        _board = board as Stage1BlackBoard;
    }

    public override SeqStates CheckLeaf(PlayerStates ps)
    {
        if (ps == PlayerStates.Skill && _seqStates == SeqStates.Fail)
        {
            Enter();
            _seqStates = SeqStates.Running;
        }

        else if (_seqStates == SeqStates.Running && ps == PlayerStates.Landing)
        {
            Exit();
            _seqStates = SeqStates.Fail;
        }

        return _seqStates;
    }

    protected override void Enter()
    {
        _blackBoard.PlayerState = State;
        _blackBoard.RD.velocity = new Vector2(_blackBoard.PA.Flip * 0.5f, 0.866f).normalized * _board.Data.ChargePower[0];
        _blackBoard.PA.ChangeAnim(PlayerStates.Jump);
    }

    public override void Exit()
    {
        _blackBoard.PA.ChangeAnim(PlayerStates.Landing);
    }

    public override void Update()
    {

    }
}