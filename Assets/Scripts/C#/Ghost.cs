using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    private Rigidbody2D _rb;

    [SerializeField, Header("Player")]
    private Transform _player;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
       _rb.velocity = (_player.position - transform.position).normalized;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform == _player)
        {
            _rb.velocity = Vector2.zero;
            enabled = false;
            gameObject.SetActive(false);
        }
    }
}
