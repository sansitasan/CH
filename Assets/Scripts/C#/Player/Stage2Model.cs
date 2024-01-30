using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Stage2Model : PlayerModel
{
    [SerializeField]
    private WallBlink _wall;

    [SerializeField]
    private Stage2Data _data;

    private bool _bSkill;

    public override void Init(StageData so)
    {
        _pa = new Player2DAnim(transform.GetChild(0).GetComponent<Animator>(), transform.GetChild(1).GetComponent<Animator>(), transform.GetChild(0).GetComponent<SpriteRenderer>());
        base.Init(so);
        _disposeList.Add(_wall);
    }

    protected override void DataInit(StageData so)
    {
        _data = so as Stage2Data;
    }

    protected override void MakeBT(StageData so)
    {
        _tree = new BehaviourTree();
        _blackBoard = new BlackBoard(transform, _pa, _rb, _tree, so);

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
