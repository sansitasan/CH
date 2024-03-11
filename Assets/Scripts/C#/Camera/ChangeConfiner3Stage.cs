using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeConfiner3Stage : MonoBehaviour
{
    private CinemachineConfiner2D _confiner;

    void Awake()
    {
        _confiner = GetComponent<CinemachineConfiner2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            _confiner.m_BoundingShape2D = collision;
        }
    }
}
