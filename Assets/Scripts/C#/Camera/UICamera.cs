using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class UICamera : MonoBehaviour
{
    public static Camera Camera { get; private set; } 

    private void Awake()
    {
        if (Camera == null)
        {
            AwakeInit();
        }
    }

    private void Start()
    {
        StartInit();
    }

    private void AwakeInit()
    {
        Camera = GetComponent<Camera>();
    }

    private void StartInit()
    {
        var camData = MainCamera.Camera.GetUniversalAdditionalCameraData();
        if (camData.cameraStack.Count < 1)
            camData.cameraStack.Add(Camera);
    }
}
