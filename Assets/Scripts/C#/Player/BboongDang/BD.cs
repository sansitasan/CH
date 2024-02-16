using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BD : MonoBehaviour, IDisposable
{
    private PlayerStates _ps;
    private Vector2 _curLookDir;
    private Rigidbody2D _rb;

    private Queue<Vector3> _posQueue;
    private Queue<Vector2> _dirQueue;
    private BehaviourTree _tree;
    private BlackBoard _blackBoard;
    private PlayerAnim _pa;

    private List<IDisposable> _disposeList = new List<IDisposable>();

    public void Init(StageData data)
    {
        _ps = PlayerStates.Idle;
        _rb = GetComponent<Rigidbody2D>();
        _posQueue = new Queue<Vector3>(10);
        _dirQueue = new Queue<Vector2>(10);
        //_pa = new PlayerAnim(gameObject)
        MakeBT(data);
    }

    private void MakeBT(StageData so)
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

    private void FixedUpdate()
    {
        _tree.Update();
    }

    private void CheckDir()
    {
        if (_posQueue.Count < 0) return;

        if (Vector3.Distance(_posQueue.Peek(), transform.position) < 0.5f)
        {
            _posQueue.Dequeue();
            _curLookDir = _dirQueue.Dequeue();
        }
    }

    public void ChangeDir(Vector3 pos, Vector2 dir)
    {
        _posQueue.Enqueue(pos);
        _dirQueue.Enqueue(dir);
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
