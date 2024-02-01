using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

public class Stage2Model : PlayerModel
{
    [SerializeField]
    private Stage2Data _data;

    [SerializeField]
    private Light2D _light;

    private bool _bCool;

    public override void Init(StageData so)
    {
        _pa = new Player2DAnim(transform.GetChild(0).GetComponent<Animator>(), transform.GetChild(1).GetComponent<Animator>(), transform.GetChild(0).GetComponent<SpriteRenderer>());
        base.Init(so);
        _light = GetComponentInChildren<Light2D>();
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

        else if (!_bCool)
        {
            Skill().Forget();
            _bCool = true;
            SkillCoolTimeCheck().Forget();
        }
    }

    private async UniTask Skill()
    {
        float time = 0;
        float basicSize = _light.pointLightOuterRadius;

        while (time < _data.FadeInTime)
        {
            await UniTask.DelayFrame(1, cancellationToken: _cts.Token);
            _light.pointLightOuterRadius = basicSize + (_data.MaxCircleSize - basicSize) * time/_data.FadeInTime;
            time += Time.deltaTime;
        }

        _light.pointLightOuterRadius = _data.MaxCircleSize;
        time = 0;
        while (time < _data.LightOnTime)
        {
            await UniTask.DelayFrame(1, cancellationToken: _cts.Token);
            time += Time.deltaTime;
        }

        time = 0;
        while (time < _data.FadeOutTime)
        {
            await UniTask.DelayFrame(1, cancellationToken: _cts.Token);
            _light.pointLightOuterRadius = _data.MaxCircleSize - (_data.MaxCircleSize - basicSize) * time / _data.FadeInTime;
            time += Time.deltaTime;
        }

        _light.pointLightOuterRadius = basicSize;
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

    private void FixedUpdate()
    {
        _tree.Update();
    }

    public override void EditInit(StageData so)
    {
        _light = GetComponentInChildren<Light2D>();
        _light.pointLightOuterRadius = _data.OuterCircleSize;
        _light.pointLightInnerRadius = _data.InnerCircleSize;
        _light.intensity = _data.Intensity;
        _light.falloffIntensity = _data.Intensity;
    }
}
