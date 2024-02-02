using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class Stage1Model : PlayerModel
{
    private int _rayMask;
    [SerializeField]
    private Stage1Data _data;

    private bool _bCheck;
    private bool _bCool;

    public override void Init(StageData so)
    {
        _pa = new Player1DAnim(transform.GetChild(0).GetComponent<Animator>(), transform.GetChild(1).GetComponent<Animator>(), transform.GetChild(0).GetComponent<SpriteRenderer>());
        base.Init(so);
        _rayMask = LayerMask.GetMask("Ground");
    }

    protected override void DataInit(StageData so)
    {
        _data = so as Stage1Data;

        _rb.gravityScale = _data.GravityScale;
        _rb.sharedMaterial = _data.GroundMaterial;
    }

    protected override void MakeBT(StageData so)
    {
        _tree = new BehaviourTree();
        _blackBoard = new Stage1BlackBoard(transform, _pa, _rb, _tree, so);

        var skillSeq = new BehaviourSequence();
        var skillNode = new BehaviourNormalSelector();
        var skillLeaf = new SkillLeaf(_blackBoard);
        skillNode.AddNode(skillLeaf);
        skillSeq.AddSequenceNode(skillNode);
        _tree.AddSeq(skillSeq);

        var skySeq = new BehaviourSequence();
        var skyNode = new BehaviourNormalSelector();
        var skyLeaf = new SkyLeaf(_blackBoard);
        skyNode.AddNode(skyLeaf);
        skySeq.AddSequenceNode(skyNode);
        _tree.AddSeq(skySeq);

        var behaveSeq = new BehaviourSequence();
        var behaveNode = new BehaviourNormalSelector();
        var behaveLeaf = new Stage1BehaveLeaf(_blackBoard);
        behaveNode.AddNode(behaveLeaf);
        behaveSeq.AddSequenceNode(behaveNode);
        _tree.AddSeq(behaveSeq);

        var moveSeq = new BehaviourSequence();
        var moveNode = new BehaviourNormalSelector();
        var moveLeaf = new Stage1MoveLeaf(_blackBoard);
        var idleLeaf = new IdleLeaf(_blackBoard);
        moveNode.AddNode(moveLeaf);
        moveNode.AddNode(idleLeaf);
        moveSeq.AddSequenceNode(moveNode);
        _tree.AddSeq(moveSeq);

        _disposeList.Add(_tree);
        _disposeList.Add(_blackBoard);
        _tree.CheckSeq(PlayerStates.Idle);
    }

    private void FixedUpdate()
    {
        _tree.Update();
        if (!_bCheck)
            CheckGround().Forget();
    }

    public override void PlayerInput(PlayerStates state)
    {
        if (state != PlayerStates.Skill)
            base.PlayerInput(state);

        else if (!_bCool)
        {
            base.PlayerInput(state);
            _bCool = true;
            SkillCoolTimeCheck().Forget();
        }
    }

    public override void PlayerInput(PlayerStates state, Vector2 vector)
    {
        if (vector == Vector2.up || vector == Vector2.down)
        {
            _blackBoard.MoveDir = vector;
            return;
        }
        base.PlayerInput(state, vector);
    }

    private async UniTask SkillCoolTimeCheck()
    {
        float time = 0;
        while (time < _data.SkillCoolTime)
        {
            await UniTask.DelayFrame(1, cancellationToken: _cts.Token);
            time += Time.deltaTime;
        }

        _bCool = false;
    }

    private async UniTask CheckGround()
    {
        _bCheck = true;
        if (_blackBoard.PlayerState == PlayerStates.Behave || _blackBoard.PlayerState == PlayerStates.Skill)
        {
            await UniTask.WaitUntil(() => _blackBoard.PlayerState != PlayerStates.Behave, cancellationToken: _cts.Token);

            await UniTask.DelayFrame(30, cancellationToken: _cts.Token);
        }
        
        Vector2 left = _rb.position - new Vector2(0.45f, 1.125f);
        Vector2 right = _rb.position - new Vector2(-0.45f, 1.125f);

        RaycastHit2D leftRay = Physics2D.Raycast(left, Vector2.down, 0.1f, _rayMask);
        RaycastHit2D rightRay = Physics2D.Raycast(right, Vector2.down, 0.1f, _rayMask);
        Debug.DrawLine(left, left - Vector2.down * 0.1f, Color.red, 1);
        Debug.DrawLine(right, right - Vector2.down * 0.1f, Color.red, 1);

        if (leftRay.collider == null && rightRay.collider == null)
        {
            _rb.sharedMaterial = _data.WallMaterial;
            _tree.CheckSeq(PlayerStates.InTheSky);
        }

        else
        {
            _rb.sharedMaterial = _data.GroundMaterial;
            _tree.CheckSeq(PlayerStates.Landing);
        }
        _bCheck = false;
    }

    public override void EditInit(StageData so)
    {
    }
}