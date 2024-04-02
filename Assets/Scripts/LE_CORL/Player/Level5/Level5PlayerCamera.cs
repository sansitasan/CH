using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level5PlayerCamera : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] float farFrom;
    float fixedY;

    private void Awake()
    {
        fixedY = transform.position.y;
    }

    void Update()
    {
        transform.position =
            new Vector3(player.position.x + farFrom, fixedY, -10);
    }
}
