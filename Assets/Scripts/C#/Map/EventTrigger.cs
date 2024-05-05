using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EventTypes
{
    None,
    Start,
    Middle,
    End,
    Dead
}

public class EventTrigger : MonoBehaviour
{
    public EventTypes Type;

    private GameScene _scene;

    private void Awake()
    {
        _scene = transform.parent.GetComponent<GameScene>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _scene.GetEvent(Type);
            gameObject.SetActive(false);
        }
    }
}
