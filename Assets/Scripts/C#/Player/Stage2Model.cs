using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Stage2Model : PlayerModel
{
    [SerializeField]
    private Stage2Data _data;

    [SerializeField]
    private Light2D _light;
    [SerializeField]
    private Animator _skillAnim;
    [SerializeField]
    private Image _skillImg;
    [SerializeField]
    private BD _bd;
    private Character2DAnim _pa;

    private float _skillCoolTime;
    private bool _bCool;

    public override void Init(StageData so)
    {
        _pa = new Character2DAnim(transform.GetChild(0).gameObject);
        _disposeList.Add(_pa);
        base.Init(so);
        _light = GetComponentInChildren<Light2D>(true);
        _skillCoolTime = _skillAnim.runtimeAnimatorController.animationClips[0].length;
    }

    protected override void DataInit(StageData so)
    {
        _data = so as Stage2Data;
    }

    protected override void MakeBT(StageData so)
    {
        _tree = new BehaviourTree();
        _blackBoard = new Stage2BlackBoard(transform, _pa, _rb, _tree, so, _bd);

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

        await SkillEffectAsync();

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

    private async UniTask SkillEffectAsync()
    {
        float time = 0;
        Time.timeScale = 0;
        _bd.UseSkill(_data.SkillCoolTime);
        DisableInput(true);
        _skillAnim.gameObject.SetActive(true);
        _skillAnim.Play("Skill", 0, 0);

        while (time < _skillCoolTime * 2)
        {
            time += Time.unscaledDeltaTime;
            _skillImg.color = Color.Lerp(new Color(1, 1, 1, 0), Color.white, time);
            await UniTask.DelayFrame(1, cancellationToken: _cts.Token);
        }
        _skillImg.color = Color.white;
        time = 0;
        while (time < _skillCoolTime * 3)
        {
            time += Time.unscaledDeltaTime;
            await UniTask.DelayFrame(1, cancellationToken: _cts.Token);
        }

        time = 0;
        while (time < _skillCoolTime * 2)
        {
            time += Time.unscaledDeltaTime;
            _skillImg.color = Color.Lerp(Color.white, new Color(1, 1, 1, 0), time);
            await UniTask.DelayFrame(1, cancellationToken: _cts.Token);
        }

        _skillImg.color = new Color(1, 1, 1, 0);
        _skillAnim.gameObject.SetActive(false);

        await UniTask.DelayFrame(15, cancellationToken: _cts.Token);

        Time.timeScale = 1;
        DisableInput(false);
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

    public override async UniTask AfterScriptInit()
    {
        await _pa.StartFadeAsync();
    }
}
