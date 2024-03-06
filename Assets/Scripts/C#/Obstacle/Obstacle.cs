using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour, IInteractable
{
    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (!_rb.isKinematic && _rb.velocity.magnitude > 2f)
        {
            _rb.AddForce(-_rb.velocity.normalized * _rb.mass * 100, ForceMode2D.Force);
        }
    }

    public bool Interact(Vector3 dir)
    {
        if (_rb.isKinematic)
        {
            Push(dir);
        }

        return true;
    }

    private void Push(Vector3 dir)
    {
        _rb.isKinematic = false;
        _rb.AddForce(dir * _rb.mass * 21.2f, ForceMode2D.Impulse);
        CheckStop().Forget();
    }

    private async UniTask CheckStop()
    {
        await UniTask.WaitUntil(() => _rb.velocity.magnitude < 1.2f);
        _rb.velocity = Vector2.zero;
        _rb.isKinematic = true;
        transform.position = Vector3Int.RoundToInt(transform.position);
    }
}
