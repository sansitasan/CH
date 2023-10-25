using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BD : MonoBehaviour
{
    private Collider2D _trigger;
    private CancellationTokenSource _cts = new CancellationTokenSource();

    private void Awake()
    {
        _trigger = GetComponent<Collider2D>();
    }

    public void Skill(bool bActive)
    {
        _trigger.enabled = bActive;
    }

    private async UniTaskVoid TriggerOnOff()
    {
        _trigger.enabled = true;

        await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: _cts.Token);

        _trigger.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable))
            interactable.Interact(transform);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable))
            interactable.Interact(null);
    }
}
