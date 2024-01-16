using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeConfiner : MonoBehaviour
{
    private CinemachineConfiner2D _confiner;

    private Collider2D _prevCollider;

    void Awake()
    {
        _confiner = GetComponent<CinemachineConfiner2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            _prevCollider = _confiner.m_BoundingShape2D;
            _confiner.m_BoundingShape2D = collision;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8 && collision != _prevCollider)
        {
            _confiner.m_BoundingShape2D = _prevCollider;
            _prevCollider = collision;
        }
    }
}
