using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Stage2Model : PlayerModel
{
    [SerializeField]
    private WallBlink _wall;

    private bool _bSkill;

    public override void Init()
    {
        base.Init();
        _disposeList.Add(_wall);
    }

    protected override void MakeBT()
    {
        _tree = new BehaviourTree();
        _blackBoard = new BlackBoard(transform, _pa, _rb, _tree, _speed);

        var moveSeq = new BehaviourSequence();
        var moveNode = new BehaviourNormalSelector();
        var moveLeaf = new Stage2MoveLeaf(_blackBoard);
        var idleLeaf = new IdleLeaf(_blackBoard);
        moveNode.AddNode(moveLeaf);
        moveNode.AddNode(idleLeaf);
        moveSeq.AddSequenceNode(moveNode);
        _tree.AddSeq(moveSeq);

        _disposeList.Add(_tree);
        _disposeList.Add(_blackBoard);
        _tree.CheckSeq(PlayerStates.Idle);
    }

    public override void PlayerInput(PlayerStates state)
    {
        if (state != PlayerStates.Skill)
            base.PlayerInput(state);
        else
            Skill();
    }

    private void Skill()
    {
        if (!_bSkill)
        {
            _bSkill = true;
            SkillAsync().Forget();
        }
    }

    private async UniTaskVoid SkillAsync()
    {
        await _wall.BlinkAsync();
        _bSkill = false;
    }

    private void FixedUpdate()
    {
        _tree.Update();
    }
}
