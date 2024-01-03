using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour, IInteractable
{
    private bool _bPush = true;
    private List<Transform> _nearObstacles = new List<Transform>(10);
    private readonly string _sObstacle = "Obstacle";

    public bool Interact(Vector3 dir)
    {
        if (_bPush)
            return Knockback(dir);
        return false;
    }

    private bool Knockback(Vector3 dir)
    {
        if (CheckForward(dir))
        {
            _bPush = false;
            DOTween.Sequence()
                .AppendCallback(() => transform.DOMove(transform.position + dir, 0.2f).SetEase(Ease.OutCubic))
                .AppendCallback(() => _bPush = true);
            return true;
        }
        return false;
    }

    private bool CheckForward(Vector3 dir)
    {
        int count = _nearObstacles.Count;
        for (int i = 0; i < count; ++i)
        {
            if (Vector3.Distance(_nearObstacles[i].position, transform.position + dir) < 0.1f)
            {
                return false;
            }
        }
        return true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(_sObstacle))
            _nearObstacles.Add(collision.transform);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(_sObstacle))
            _nearObstacles.Remove(collision.transform);
    }
}
