using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserObstacle : MonoBehaviour
{
    private Collider2D _col;
    private List<Transform> _transforms = new List<Transform>();

    void Awake()
    {
        _col = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("X");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

    }
}
