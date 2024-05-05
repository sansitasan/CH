using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[ExecuteInEditMode]
public class MainCamera : MonoBehaviour
{
    public static Camera Camera { get; private set; }

    private static int _prevCullingMask;

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
        _prevCullingMask = Camera.cullingMask;
    }

    public static void SetMask(int mask)
    {
        if (_prevCullingMask == Camera.cullingMask)
            Camera.cullingMask = 1 << mask;
        else
            Camera.cullingMask = _prevCullingMask;
    }
}
