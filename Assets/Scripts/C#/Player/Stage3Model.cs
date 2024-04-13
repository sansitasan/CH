using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Stage3Model : PlayerModel
{
    private List<Transform> _obstacles = new List<Transform>(8);

    private bool _bSkill;
    [SerializeField]
    private Stage3Data _data;
    [SerializeField]
    private BD _bd;

    [SerializeField]
    private TextMeshProUGUI _countText;
    private Character2DAnim _pa;
    private int _levelCount;
    private int _skillCount;

    public override void Init(StageData so)
    {
        _pa = new Character2DAnim(transform.GetChild(0).gameObject);
        base.Init(so);
        _disposeList.Add(_pa);
    }

    public override void PlayerInput(PlayerStates state)
    {
        if (state != PlayerStates.Skill)
            base.PlayerInput(state);
        else
            Skill();
    }

    public override void EditInit(StageData so)
    {
    }

    public override async UniTask AfterScriptInit()
    {
        await _pa.StartFadeAsync();
    }

    public override void Dispose()
    {
        _pa = null;
        base.Dispose();
    }

    protected override void DataInit(StageData so)
    {
        ++_levelCount;
        _data = so as Stage3Data;
        if (_levelCount == 1)
            _skillCount = _data.MoveCount_1;
        else if (_levelCount == 2)
            _skillCount = _data.MoveCount_2;
        _countText.text = _skillCount.ToString();
    }

    protected override void MakeBT(StageData so)
    {
        _tree = new BehaviourTree();
        _blackBoard = new Stage3BlackBoard(transform, _pa, _rb, _tree, so, _bd);

        var deadLeaf = new DeadLeaf(_blackBoard);

        var moveSeq = new BehaviourSequence();
        var moveNode = new BehaviourNormalSelector();
        var moveLeaf = new Stage2MoveLeaf(_blackBoard);
        var idleLeaf = new IdleLeaf(_blackBoard);
        moveNode.AddNode(deadLeaf);
        moveNode.AddNode(moveLeaf);
        moveNode.AddNode(idleLeaf);
        moveSeq.AddSequenceNode(moveNode);
        _tree.AddSeq(moveSeq);

        _disposeList.Add(_tree);
        _disposeList.Add(_blackBoard);
        _tree.CheckSeq(PlayerStates.Idle);
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
        int count = _obstacles.Count;

        if (count > 0)
        {
            float dis = 2.24f;
            Vector3 temp;
            Transform near = null;
            for (int i = 0; i < count; ++i)
            {
                temp = _obstacles[i].position - transform.position;
                if (dis > temp.magnitude && Vector3.Dot(_pa.LookDir, temp.normalized) > 0.708f)
                {
                    dis = temp.magnitude;
                    near = _obstacles[i];
                }
            }

            if (near != null)
            {
                _countText.text = (--_skillCount).ToString();
                bool t = near.GetComponent<IInteractable>().Interact(_pa.LookDir);
                if (t)
                {
                    DisableInput(true);
                    _pa.ChangeAnim(PlayerStates.Skill);
                    _bd.UseSkill(true);
                }
                await UniTask.DelayFrame(10);
                _pa.ChangeAnim(PlayerStates.Idle);
                await UniTask.DelayFrame(20);
                DisableInput(false);
                _bd.UseSkill(false);
                await UniTask.DelayFrame(30);
            }
        }
        _bSkill = false;
    }

    private void FixedUpdate()
    {
        _tree.Update();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IInteractable>(out var com))
            _obstacles.Add(collision.transform);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IInteractable>(out var com))
            _obstacles.Remove(collision.transform);
    }
}
