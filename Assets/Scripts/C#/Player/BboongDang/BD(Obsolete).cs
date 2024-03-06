using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

//public class BD : MonoBehaviour, IDisposable
//{
//    private Rigidbody2D _rb;
//
//    [SerializeField]
//    private BehaviourTree _tree;
//    [SerializeField]
//    private BlackBoard _blackBoard;
//    private CharacterAnim _pa;
//    [SerializeField]
//    private Transform _player;
//    private Collider2D _col;
//
//    private List<IDisposable> _disposeList = new List<IDisposable>();
//
//    public void Init(StageData data)
//    {
//        _rb = GetComponent<Rigidbody2D>();
//        _col = GetComponent<Collider2D>();
//        _pa = new Character2DAnim(gameObject);
//        MakeBT(data);
//    }
//
//    private void MakeBT(StageData so)
//    {
//        _tree = new BehaviourTree();
//        _blackBoard = new Stage2BlackBoard(transform, _pa, _rb, _tree, so);
//
//        var moveSeq = new BehaviourSequence();
//        var moveNode = new BehaviourNormalSelector();
//        var moveLeaf = new Stage2MoveLeaf(_blackBoard);
//        var idleLeaf = new IdleLeaf(_blackBoard);
//        moveNode.AddNode(moveLeaf);
//        moveNode.AddNode(idleLeaf);
//        moveSeq.AddSequenceNode(moveNode);
//        _tree.AddSeq(moveSeq);
//
//        _disposeList.Add(_tree);
//        _disposeList.Add(_blackBoard);
//        _blackBoard.MoveDir = Vector2.down;
//        _tree.CheckSeq(PlayerStates.Idle);
//    }
//
//    private void Update()
//    {
//        Vector3 dir = (_player.position - transform.position).normalized;
//        float dis = (_player.position - transform.position).magnitude;
//
//        if (dis > 1.5f)
//        {
//            _blackBoard.MoveDir = dir;
//            if (dis >= 10f)
//            {
//                DisableCollider().Forget();
//            }
//            _tree.CheckSeq(PlayerStates.Move);
//        }
//
//        else 
//        {
//            _tree.CheckSeq(PlayerStates.Idle);
//        }
//    }
//
//    private async UniTask DisableCollider()
//    {
//        _col.enabled = false;
//        while ((_player.position - transform.position).magnitude > 1.5f)
//        {
//            await UniTask.DelayFrame(1);
//        }
//        _col.enabled = true;
//    }
//
//    private void FixedUpdate()
//    {
//        _tree.Update();
//    }
//
//    public void UseSkill(float time)
//    {
//        _pa.UseSkill(time).Forget();
//    }
//
//    public async UniTask AfterScriptInit()
//    {
//        await _pa.StartFadeAsync();
//    }
//
//    public void Dispose()
//    {
//        foreach (var dispose in _disposeList)
//        {
//            dispose.Dispose();
//        }
//    }
//}
