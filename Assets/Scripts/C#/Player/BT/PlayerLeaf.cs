using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class IdleLeaf : BehaviourLeaf
{
    public IdleLeaf(BlackBoard board) : base(board, PlayerStates.Idle) 
    {

    }

    public override SeqStates CheckLeaf(PlayerStates ps)
    {
        Enter();
        return SeqStates.Running;
    }

    protected override void Enter()
    {
        base.Enter();
        _blackBoard.CA.StopLoopSound();
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
    private AudioClip _grassClip;
    private AudioClip _rockClip;

    public MoveLeaf(BlackBoard board) : base(board, PlayerStates.Move) 
    {
        LResourcesManager.TryGetSoundClip(ESoundType.Stage1_Grass_Move, out _grassClip);
        LResourcesManager.TryGetSoundClip(ESoundType.Stage1_Rock_Move, out _rockClip);
    }

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
        if (_blackBoard.Player.position.y < 54)
            _blackBoard.CA.PlaySound(_grassClip, ESound.Bgm);
        else
            _blackBoard.CA.PlaySound(_rockClip, ESound.Bgm);
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
        _blackBoard.RD.velocity = new Vector2(_blackBoard.MoveDir.x, 0).normalized 
            * _blackBoard.Data.Speed;
    }

    public override void Exit()
    {
        //_blackBoard.RD.velocity = Vector2.zero;
    }
}

public class Stage2MoveLeaf : MoveLeaf
{
    private BlackBoard2D _bB;

    private struct MoveData
    {
        public readonly Vector3 Position;
        public readonly Vector2 MoveDir;

        public MoveData(Vector3 position, Vector2 moveDir)
        {
            Position = position;
            MoveDir = moveDir;
        }
    }

    private Queue<MoveData> _moveQueue = new Queue<MoveData>(20);

    public Stage2MoveLeaf(BlackBoard board) : base(board) 
    {
        _bB = board as BlackBoard2D;
        for (int i = 0; i < 10; ++i)
        {
            _moveQueue.Enqueue(new MoveData(_blackBoard.Player.position - Vector3.right * (1f - (float)i / 10), Vector2.right));
        }
    }

    protected override void Enter()
    {
        base.Enter();
        _bB.BD.UpdateAnim(PlayerStates.Move);
    }

    public override void Exit()
    {
        _bB.BD.UpdateAnim(PlayerStates.Idle);
        _blackBoard.RD.velocity = Vector2.zero;
    }

    public override void Update()
    {
        MoveData data = _moveQueue.Dequeue();
        _moveQueue.Enqueue(new MoveData(_blackBoard.Player.position, _blackBoard.MoveDir));
        _bB.BD.SetMoveData(data.Position, data.MoveDir);
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
        BehaveAsync().Forget();
    }

    private async UniTask BehaveAsync()
    {
        await UniTask.DelayFrame(1);
        _blackBoard.Tree.CheckSeq(PlayerStates.InTheSky2);
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
    private AudioClip _skyClip;
    private AudioClip _grassLandClip;
    private AudioClip _rockLandClip;

    public SkyLeaf(BlackBoard board) : base(board, PlayerStates.InTheSky) 
    {
        LResourcesManager.TryGetSoundClip(ESoundType.Stage1_Jump, out _skyClip);
        LResourcesManager.TryGetSoundClip(ESoundType.Stage1_Grass_Land, out _grassLandClip);
        LResourcesManager.TryGetSoundClip(ESoundType.Stage1_Rock_Land, out _rockLandClip);
    }

    public override SeqStates CheckLeaf(PlayerStates ps)
    {
        if ((ps == PlayerStates.InTheSky || ps == PlayerStates.InTheSky2) && _seqStates == SeqStates.Fail)
        {
            _seqStates = SeqStates.Running;
            Enter();
            _blackBoard.PA.ChangeAnim(PlayerStates.Jump);
            if (ps == PlayerStates.InTheSky2)
                _blackBoard.CA.PlaySound(_skyClip);
            else
                _blackBoard.CA.StopLoopSound();
        }

        else if (_seqStates == SeqStates.Running && ps == PlayerStates.Landing)
        {
            Exit();
        }

        return _seqStates;
    }

    protected override void Enter()
    {
        base.Enter();
        _seqStates = SeqStates.Running;
        _blackBoard.PA.ChangeAnim(PlayerStates.Jump);
    }

    public override void Exit()
    {
        _blackBoard.PA.ChangeAnim(PlayerStates.Landing);
        _blackBoard.PlayerState = PlayerStates.Landing;
        if (_blackBoard.Player.position.y < 54)
            _blackBoard.CA.PlaySound(_grassLandClip);
        else
            _blackBoard.CA.PlaySound(_rockLandClip);
        _seqStates = SeqStates.Fail;
    }

    private async UniTask ExitAsync()
    {
        await UniTask.DelayFrame(2);
        if (_blackBoard.Player.position.y < 54)
            _blackBoard.CA.PlaySound(_grassLandClip);
        else
            _blackBoard.CA.PlaySound(_rockLandClip);
    }

    public override void Update()
    {

    }
}

public class SkillLeaf : BehaviourLeaf
{
    private Stage1BlackBoard _board;
    private AudioClip _skyClip;
    private AudioClip _grassLandClip;
    private AudioClip _rockLandClip;

    public SkillLeaf(BlackBoard board) : base(board, PlayerStates.Skill)
    {
        _board = board as Stage1BlackBoard;
        LResourcesManager.TryGetSoundClip(ESoundType.Stage1_Jump, out _skyClip);
        LResourcesManager.TryGetSoundClip(ESoundType.Stage1_Grass_Land, out _grassLandClip);
        LResourcesManager.TryGetSoundClip(ESoundType.Stage1_Rock_Land, out _rockLandClip);
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
        base.Enter();
        _blackBoard.PlayerState = State;
        _blackBoard.RD.velocity = new Vector2(_blackBoard.PA.Flip * 0.5f, 0.866f).normalized * _board.Data.ChargePower[0];
        _blackBoard.PA.ChangeAnim(PlayerStates.Jump);
        _blackBoard.CA.PlaySound(_skyClip);
    }

    public override void Exit()
    {
        _blackBoard.PA.ChangeAnim(PlayerStates.Landing);
        if (_blackBoard.Player.position.y < 54)
            _blackBoard.CA.PlaySound(_grassLandClip);
        else
            _blackBoard.CA.PlaySound(_rockLandClip);
    }

    public override void Update()
    {

    }
}

public class DeadLeaf : BehaviourLeaf
{
    public DeadLeaf(BlackBoard board) : base(board)
    {

    }

    public override SeqStates CheckLeaf(PlayerStates ps)
    {
        if (ps == PlayerStates.Dead) {
            Enter();
            _seqStates = SeqStates.Running;
        }

        else
            _seqStates = SeqStates.Fail;

        return _seqStates;
    }

    protected override void Enter()
    {
        GameScene.Instance.GetEvent(EventTypes.Dead);
    }

    public override void Exit()
    {

    }

    public override void Update()
    {

    }
}