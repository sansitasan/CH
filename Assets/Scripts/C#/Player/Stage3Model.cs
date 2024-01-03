using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Stage3Model : PlayerModel
{
    private List<Transform> _obstacles = new List<Transform>(8);

    private bool _bSkill;
    private readonly string _sObstacle = "Obstacle";

    protected override void Init()
    {
        base.Init();
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
        int count = _obstacles.Count;
        if (count > 0 && _blackBoard.MoveDir != Vector2.zero)
        {
            float dis = 1.4f;
            Vector3 temp;
            Transform near = null;
            for (int i = 0; i < count; ++i)
            {
                temp = _obstacles[i].position - transform.position;
                if (dis > temp.magnitude && Vector3.Dot(_blackBoard.MoveDir, temp) > 0.708f)
                {
                    dis = temp.magnitude;
                    near = _obstacles[i];
                }
            }

            if (near != null)
            {
                bool t = near.GetComponent<IInteractable>().Interact(_blackBoard.MoveDir);
                Debug.Log(t);
                await UniTask.DelayFrame(60);
            }
            _bSkill = false;
        }
    }

    private void FixedUpdate()
    {
        _tree.Update();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(_sObstacle))
            _obstacles.Add(collision.transform);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(_sObstacle))
            _obstacles.Remove(collision.transform);
    }
}
