using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using UniRx;
using UnityEngine;

public class Stage1Model : PlayerModel
{
    private int _rayMask;
    [SerializeField]
    private Stage1Data _data;
    [SerializeField]
    private Animator _skillAnim;
    private Character1DAnim _pa;

    private CustomAudio _ca;
    private Dictionary<ESoundType, AudioClip> _clips = new Dictionary<ESoundType, AudioClip>();

    private bool _bCheck;

    public override void Init(StageData so)
    {
        _rb = GetComponent<Rigidbody2D>();
        _pa = new Character1DAnim(transform.GetChild(0).gameObject, transform.GetChild(1).gameObject, _rb);
        base.Init(so);
        _disposeList.Add(_pa);
        _rayMask = LayerMask.GetMask("Ground");
        //LResourcesManager.TryGetSoundClip(ESoundType.Stage1_Jump, out var jump);
        //_clips.Add(ESoundType.Stage1_Jump, jump);
        //LResourcesManager.TryGetSoundClip(ESoundType.Stage1_Grass_Move, out var gm);
        //_clips.Add(ESoundType.Stage1_Grass_Move, gm);
        //LResourcesManager.TryGetSoundClip(ESoundType.Stage1_Grass_Land, out var gl);
        //_clips.Add(ESoundType.Stage1_Grass_Land, gl);
        //LResourcesManager.TryGetSoundClip(ESoundType.Stage1_Rock_Move, out var rm);
        //_clips.Add(ESoundType.Stage1_Rock_Move, gm);
        //LResourcesManager.TryGetSoundClip(ESoundType.Stage1_Rock_Land, out var rl);
        //_clips.Add(ESoundType.Stage1_Rock_Land, rl);
        //_ca = new CustomAudio(GetComponent<AudioSource>(), ESound.Effect);
        //
        //this.ObserveEveryValueChanged(_ => _blackBoard.PlayerState).Subscribe(_ =>
        //{
        //    switch (_blackBoard.PlayerState)
        //    {
        //        case PlayerStates.Idle:
        //            _ca.StopSound(); break;
        //
        //        case PlayerStates.Move:
        //            if (transform.position.y < 54)
        //                _ca.PlaySound(_clips[ESoundType.Stage1_Grass_Move], ESound.Bgm);
        //            else
        //                _ca.PlaySound(_clips[ESoundType.Stage1_Rock_Move], ESound.Bgm);
        //            break;
        //
        //        case PlayerStates.InTheSky:
        //            _ca.PlaySound(_clips[ESoundType.Stage1_Jump]);
        //            break;
        //
        //        case PlayerStates.Skill:
        //            _ca.PlaySound(_clips[ESoundType.Stage1_Jump]);
        //            break;
        //
        //        case PlayerStates.Landing:
        //            if (transform.position.y < 54)
        //                _ca.PlaySound(_clips[ESoundType.Stage1_Grass_Land]);
        //            else
        //                _ca.PlaySound(_clips[ESoundType.Stage1_Rock_Land]);
        //            break;
        //    }
        //});
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
        _tree?.Update();
        if (!_bCheck)
            CheckGround().Forget();
    }

    public override void PlayerInput(PlayerStates state)
    {
        if (state != PlayerStates.Skill)
            base.PlayerInput(state);

        else if (!_blackBoard.PA.BCoolTime)
        {
            SkillEffect();
            _blackBoard.PA.UseSkill(_data.SkillCoolTime).Forget();
            base.PlayerInput(state);
        }
    }

    private void SkillEffect()
    {
        //_ca.PlaySound(_clips[ESoundType.Stage1_Jump]);
        _skillAnim.gameObject.SetActive(true);
        _skillAnim.Play("Skill", 0, 0);
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

    private async UniTask CheckGround()
    {
        _bCheck = true;
        if (_blackBoard.PlayerState == PlayerStates.Behave)
        {
            await UniTask.WaitUntil(() => _blackBoard.PlayerState != PlayerStates.Behave, cancellationToken: _cts.Token);
            await UniTask.DelayFrame(30, cancellationToken: _cts.Token);
        }

        else if (_blackBoard.PlayerState == PlayerStates.Skill)
        {
            await UniTask.DelayFrame(30, cancellationToken: _cts.Token);
            _blackBoard.PlayerState = PlayerStates.Idle;
        }

        RayCastGround();

        _bCheck = false;
    }

    private bool RayCastGround()
    {
        Vector2 left = _rb.position - new Vector2(0.45f, 1.125f);
        Vector2 right = _rb.position - new Vector2(-0.45f, 1.125f);

        RaycastHit2D leftRay = Physics2D.Raycast(left, Vector2.down, 0.1f, _rayMask);
        RaycastHit2D rightRay = Physics2D.Raycast(right, Vector2.down, 0.1f, _rayMask);

        if (leftRay.collider == null && rightRay.collider == null)
        {
            _rb.sharedMaterial = _data?.WallMaterial;
            _tree.CheckSeq(PlayerStates.InTheSky);
            return true;
        }

        else
        {
            _rb.sharedMaterial = _data?.GroundMaterial;
            _tree.CheckSeq(PlayerStates.Landing);
            return false;
        }
    }

    public override void EditInit(StageData so)
    {
    }

    public override async UniTask AfterScriptInit()
    {
        await _pa.StartFadeAsync();
    }
}
