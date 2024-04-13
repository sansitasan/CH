using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[ExecuteInEditMode]
public class MainCamera : MonoBehaviour
{
    public static Camera Camera { get; private set; }

    private void Awake()
    {
        if (Camera == null)
        {
            Init();
        }
    }

    private void Init()
    {
        Camera = GetComponent<Camera>();
    }
}
