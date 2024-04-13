using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempCamera : MonoBehaviour
{
    void Awake()
    {
        Destroy(gameObject);
    }
}
