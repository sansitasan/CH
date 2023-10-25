using UnityEngine;

public class Stage1Model : PlayerModel
{
    private int _rayMask;
    [SerializeField]
    private PhysicsMaterial2D[] _materials;
    [SerializeField]
    private int[] _chargePower;

    protected override void Init()
    {
        base.Init();
        MakeBT();
        _rayMask = LayerMask.GetMask("Ground");
        _rb.sharedMaterial = _materials[0];
    }

    protected override void MakeBT()
    {
        _pa = new PlayerAnim(transform.GetChild(0).GetComponent<Animator>(), 
            transform.GetChild(1).GetComponent<Animator>());

        _tree = new BehaviourTree();
        _blackBoard = new BlackBoard(transform, _pa, _rb, _tree, _speed, _chargePower);

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

    public override void Skill()
    {
        
    }

    private void FixedUpdate()
    {
        _tree.Update();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        RaycastHit2D leftRay = Physics2D.Raycast(_rb.position - new Vector2(0.495f, 0), Vector2.down, 0.6f, _rayMask);
        RaycastHit2D rightRay = Physics2D.Raycast(_rb.position + new Vector2(0.495f, 0), Vector2.down, 0.6f, _rayMask);

        if (leftRay.collider == null && rightRay.collider == null)
        {
            _rb.sharedMaterial = _materials[1];
            _tree.CheckSeq(PlayerStates.InTheSky);
        }
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        RaycastHit2D leftRay = Physics2D.Raycast(_rb.position - new Vector2(0.495f, 0), Vector2.down, 1, _rayMask);
        RaycastHit2D rightRay = Physics2D.Raycast(_rb.position + new Vector2(0.495f, 0), Vector2.down, 1, _rayMask);

        if (leftRay.collider != null || rightRay.collider != null)
        {
            _rb.velocity = Vector2.zero;
            _rb.sharedMaterial = _materials[0];
            _tree.CheckSeq(PlayerStates.Landing);
        }
    }
}
