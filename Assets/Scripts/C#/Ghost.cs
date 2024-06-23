using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Material _mat;

    [SerializeField, Header("Player")]
    private Transform _player;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _mat = GetComponent<SpriteRenderer>().material;
    }

    private void FixedUpdate()
    {
        Vector3 vector = _player.position - transform.position;
       _rb.velocity = vector.normalized;
        _mat.SetFloat("_Distance", vector.magnitude);
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
