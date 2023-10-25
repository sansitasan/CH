using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1Block : MonoBehaviour, IInteractable
{
    private Transform _dst;

    public void Interact(Transform t)
    {
        _dst = t;
    }

    private void FixedUpdate()
    {
        if (_dst != null)
        {
            transform.position += Vector3.Normalize(_dst.position - transform.position) * 0.01f;
        }
    }
}
