using UnityEngine;

public class Stage1Model : PlayerModel
{
    private int _rayMask;
    [SerializeField]
    private PhysicsMaterial2D[] _materials;
    [SerializeField]
    private int[] _chargePower;
    [SerializeField]
    private float _chargeTime; 

    protected override void Init()
    {
        base.Init();
        _rayMask = LayerMask.GetMask("Ground");
        _rb.sharedMaterial = _materials[0];
    }

    protected override void MakeBT()
    {
        _tree = new BehaviourTree();
        _blackBoard = new Stage1BlackBoard(transform, _pa, _rb, _tree, _speed, _chargePower, _chargeTime);

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
        CheckGround();
    }

    public override void PlayerInput(PlayerStates state, Vector2 vector)
    {
        if (vector == Vector2.up || vector == Vector2.down)
        {
            return;
        }
        base.PlayerInput(state, vector);
    }

    private void CheckGround()
    {
        Vector2 left = _rb.position - new Vector2(0.485f, 1.5f);
        Vector2 right = _rb.position - new Vector2(-0.485f, 1.5f);

        RaycastHit2D leftRay = Physics2D.Raycast(left, Vector2.down, 0.1f, _rayMask);
        RaycastHit2D rightRay = Physics2D.Raycast(right, Vector2.down, 0.1f, _rayMask);
        Debug.DrawLine(left, left - Vector2.down * 0.1f, Color.red, 1);
        Debug.DrawLine(right, right - Vector2.down * 0.1f, Color.red, 1);
        if (leftRay.collider == null && rightRay.collider == null)
        {
            _rb.sharedMaterial = _materials[1];
            _tree.CheckSeq(PlayerStates.InTheSky);
        }

        else
        {
            //_rb.velocity = Vector2.zero;
            _rb.sharedMaterial = _materials[0];
            _tree.CheckSeq(PlayerStates.Landing);
        }
    }

    //private void OnCollisionExit2D(Collision2D collision)
    //{
    //    RaycastHit2D leftRay = Physics2D.Raycast(_rb.position - new Vector2(0.485f, 1.5f), Vector2.down, 0.6f, _rayMask);
    //    RaycastHit2D rightRay = Physics2D.Raycast(_rb.position - new Vector2(-0.485f, 1.5f), Vector2.down, 0.6f, _rayMask);
    //
    //    if (leftRay.collider == null && rightRay.collider == null)
    //    {
    //        _rb.sharedMaterial = _materials[1];
    //        _tree.CheckSeq(PlayerStates.InTheSky);
    //    }
    //}

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    Vector2 left = _rb.position - new Vector2(0.485f, 1.5f);
    //    Vector2 right = _rb.position - new Vector2(-0.485f, 1.5f);
    //
    //    RaycastHit2D leftRay = Physics2D.Raycast(left, Vector2.down, 0.6f, _rayMask);
    //    RaycastHit2D rightRay = Physics2D.Raycast(right, Vector2.down, 0.6f, _rayMask);
    //
    //    Debug.DrawLine(left, left - Vector2.down * 0.6f, Color.red, 1);
    //    Debug.DrawLine(right, right - Vector2.down * 0.6f, Color.red, 1);
    //
    //    if (leftRay.collider != null || rightRay.collider != null)
    //    {
    //        _rb.velocity = Vector2.zero;
    //        _rb.sharedMaterial = _materials[0];
    //        _tree.CheckSeq(PlayerStates.Landing);
    //    }
    //}
}
