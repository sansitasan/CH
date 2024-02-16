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
    [SerializeField]
    private List<BehaviourSequence> _seqList = new List<BehaviourSequence>();
    [SerializeField]
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

[Serializable]
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
        PlayerStates pstate = PlayerStates.None;
        //Debug.Log(ps);

        for (int i = 0; i < _nodeList.Count; ++i)
        {
            pstate = _nodeList[i].CheckNode(ps);

            if (pstate == PlayerStates.None)
            {
                return SeqStates.Fail;
            }

            else if (pstate != PlayerStates.Success)
            {
                CurNode = _nodeList[i];
                return SeqStates.Running;
            }
        }

        CurNode = null;
        //완료 시 초기화
        for (int i = 0; i < _nodeList.Count; ++i)
            _nodeList[i].SeqState = SeqStates.Fail;

        return SeqStates.Success;
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

    public abstract PlayerStates CheckNode(PlayerStates ps);

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
    public readonly PlayerStates State;
    public readonly string Name;

    public BehaviourLeaf(BlackBoard board) 
    {
        _seqStates = SeqStates.Fail;
        _blackBoard = board;
        Name = GetType().Name;
    }

    protected BehaviourLeaf(BlackBoard board, PlayerStates ps) : this(board)
    {
        State = ps;
    }

    public abstract SeqStates CheckLeaf(PlayerStates ps);

    public abstract void Update();

    protected virtual void Enter()
    {
        _blackBoard.PlayerState = State;
        _blackBoard.PA.ChangeAnim(State);
    }

    public abstract void Exit();

    public virtual void CancelBehaviour()
    {
        _seqStates = SeqStates.Fail;
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

    public override PlayerStates CheckNode(PlayerStates ps)
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

        if (SeqState == SeqStates.Success)
            return PlayerStates.Success;

        else if (SeqState == SeqStates.Fail)
            return PlayerStates.None;

        else
            return CurLeaf.State;
    }
}

[Serializable]
public class BlackBoard : IDisposable
{
    public CharacterAnim PA { get; private set; }
    public Rigidbody2D RD { get; private set; }
    public BehaviourTree Tree { get; private set; }
    public Transform Player { get; private set; }
    [field:SerializeField]
    public Vector2 MoveDir { get; set; }
    public PlayerStates PlayerState { get; set; }
    public StageData Data { get; private set; }

    public BlackBoard(Transform t, CharacterAnim pa, Rigidbody2D rd, BehaviourTree tree, StageData so)
    {
        PA = pa;
        RD = rd;
        Tree = tree;
        Player = t;
        Data = so;
    }

    public void Dispose()
    {
        RD = null;
        PA = null;
    }
}

public class Stage1BlackBoard : BlackBoard
{
    public new Stage1Data Data { get; private set; }

    public Stage1BlackBoard(Transform t, CharacterAnim pa, Rigidbody2D rd, BehaviourTree tree, StageData so) : base(t, pa, rd, tree, so)
    {
        Data = so as Stage1Data;
    }
}

public class Stage2BlackBoard : BlackBoard
{
    public new Stage2Data Data { get; private set; }


    public Stage2BlackBoard(Transform t, CharacterAnim pa, Rigidbody2D rd, BehaviourTree tree, StageData so) : base(t, pa, rd, tree, so)
    {
        Data = so as Stage2Data;
    }
}