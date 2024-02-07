using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorEvent : MonoBehaviour
{
    public event Action OnStart;
    public event Action OnEnd;

    private void Awake()
    {
        OnEnd += SetFalse;
    }

    public void StartFrameEvent()
    {
        OnStart?.Invoke();
    }

    public void EndFrameEvent()
    {
        OnEnd?.Invoke();
    }

    private void SetFalse()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        OnEnd -= SetFalse;
    }
}
