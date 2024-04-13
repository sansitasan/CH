using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class BaseCanvas : MonoBehaviour
{
    protected Canvas _canvas;
    private CanvasScaler _canvasScaler;

    void Awake()
    {
        EditAwakeInit();
        if (Application.isPlaying)
            Playinit();
    }

    private void Start()
    {
        EditStartInit();
    }

    protected virtual void EditAwakeInit()
    {
        _canvas = Util.GetOrAddComponent<Canvas>(gameObject, gameObject.activeSelf);
        _canvasScaler = Util.GetOrAddComponent<CanvasScaler>(gameObject, gameObject.activeSelf);
        _canvas.renderMode = RenderMode.ScreenSpaceCamera;
        _canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        _canvasScaler.referenceResolution = new Vector2(1920, 1080);
    }

    protected virtual void EditStartInit()
    {
        _canvas.worldCamera = UICamera.Camera;
    }

    protected virtual void Playinit()
    {

    }
}
