using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public enum SeqStates
{
    None,
    Running,
    Fail,
    Success
}

[Serializable]
public class BehaviourTree : IDisposable
{
    private List<BehaviourSequence> _seqList = new List<BehaviourSequence>();
    private BehaviourLeaf _curLeaf;
    public string LeafName;

    public BehaviourTree()
    {

    }

    public void AddSeq(BehaviourSequence seq)
    {
        _seqList.Add(seq);
    }

    public void CheckSeq(PlayerStates ps)
    {
        SeqStates state = SeqStates.Fail;
        for (int i = 0; i < _seqList.Count; ++i)
        {
            state = _seqList[i].CheckSeq(ps);

            if (state == SeqStates.Running)
            {
                _curLeaf = _seqList[i].CurNode.CurLeaf;
                LeafName = _curLeaf.Name;
                for (int j = i + 1; j < _seqList.Count; ++j)
                    _seqList[j].CancelSeq();

                break;
            }

            else if (state == SeqStates.Success)
                --i;
        }
    }

    public void Update()
    {
        _curLeaf.Update();
    }

    public void Dispose()
    {
        for (int i = 0; i < _seqList.Count; ++i)
        {
            _seqList[i].Dispose();
            _seqList[i] = null;
        }
    }
}

public class BehaviourSequence : IDisposable
{
    private List<BehaviourSequenceNode> _nodeList = null;

    public BehaviourSequenceNode CurNode;

    public BehaviourSequence()
    {
        _nodeList = new List<BehaviourSequenceNode>();
    }

    public void AddSequenceNode(BehaviourSequenceNode node)
    {
        _nodeList.Add(node);
    }

    public SeqStates CheckSeq(PlayerStates ps)
    {
        SeqStates state = SeqStates.Success;
        //Debug.Log(ps);

        for (int i = 0; i < _nodeList.Count; ++i)
        {
            state = _nodeList[i].CheckNode(ps);

            if (state == SeqStates.Running)
            {
                CurNode = _nodeList[i];
                return state;
            }

            if (state == SeqStates.Fail)
            {
                return state;
            }
        }

        CurNode = null;
        //완료 시 초기화
        for (int i = 0; i < _nodeList.Count; ++i)
            _nodeList[i].SeqState = SeqStates.Fail;

        return state;
    }

    public void CancelSeq()
    {
        if (CurNode != null)
        {
            CurNode.CancelNode();
            CurNode = null;
        }
    }

    public void Dispose()
    {
        CurNode = null;
        for (int i = 0; i < _nodeList.Count; ++i)
        {
            _nodeList[i].Dispose();
            _nodeList[i] = null;
        }
    }
}

public abstract class BehaviourSequenceNode : IDisposable
{
    public BehaviourLeaf CurLeaf { get; protected set; }
    protected List<BehaviourLeaf> _leafs = new List<BehaviourLeaf>();

    public SeqStates SeqState;

    public BehaviourSequenceNode()
    {
        SeqState = SeqStates.Fail;
    }

    public abstract SeqStates CheckNode(PlayerStates ps);

    public virtual void CancelNode()
    {
        if (SeqState == SeqStates.Running)
        {
            CurLeaf.CancelBehaviour();
            CurLeaf = null;
            SeqState = SeqStates.Fail;
        }
    }

    public void Dispose()
    {
        CurLeaf = null;
        for (int i = 0; i < _leafs.Count; ++i)
        {
            _leafs[i].Dispose();
            _leafs[i] = null;
        }
    }
}

public abstract class BehaviourLeaf : IDisposable
{
    protected SeqStates _seqStates;
    protected BlackBoard _blackBoard;
    public readonly string Name;

    public BehaviourLeaf(BlackBoard board) 
    {
        _seqStates = SeqStates.Fail;
        _blackBoard = board;
        Name = GetType().Name;
    }

    public abstract SeqStates CheckLeaf(PlayerStates ps);

    public abstract void Update();

    public abstract void Enter();

    public abstract void Exit();

    public virtual void CancelBehaviour()
    {
        _seqStates = SeqStates.Fail;
        Exit();
    }

    public virtual void Dispose()
    {
        _blackBoard = null;
    }
}

public class BehaviourNormalSelector : BehaviourSequenceNode
{
    public BehaviourNormalSelector() : base() { }

    public void AddNode(BehaviourLeaf leaf)
    {
        _leafs.Add(leaf);
    }

    public override SeqStates CheckNode(PlayerStates ps)
    {
        for (int i = 0; i < _leafs.Count; ++i)
        {
            SeqState = _leafs[i].CheckLeaf(ps);
            if (SeqState != SeqStates.Fail)
            {
                CurLeaf = _leafs[i];
                break;
            }
        }
        return SeqState;
    }
}

public class BlackBoard : IDisposable
{
    public PlayerAnim PA { get; private set; }
    public Rigidbody2D RD { get; private set; }
    public BehaviourTree Tree { get; private set; }
    public Transform Player { get; private set; }
    public int Speed;
    public Vector2 MoveDir { get; set; }

    public BlackBoard(Transform t, PlayerAnim pa, Rigidbody2D rd, BehaviourTree tree, int speed)
    {
        PA = pa;
        RD = rd;
        Tree = tree;
        Player = t;
        Speed = speed;
    }

    public void Dispose()
    {
        RD = null;
        PA = null;
    }
}

public class Stage1BlackBoard : BlackBoard
{
    public int[] ChargePower { get; private set; }

    public Stage1BlackBoard(Transform t, PlayerAnim pa, Rigidbody2D rd, BehaviourTree tree, int speed, int[] chargePower) : base(t, pa, rd, tree, speed)
    {
        ChargePower = chargePower;
    }
}