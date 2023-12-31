using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class BaseCanvas : MonoBehaviour
{
    protected Canvas _canvas;
    private CanvasScaler _canvasScaler;

    void Awake()
    {
        EditInit();
        if (Application.isPlaying)
            Playinit();
    }

    protected virtual void EditInit()
    {
        _canvas = Util.GetOrAddComponent<Canvas>(gameObject);
        _canvasScaler = Util.GetOrAddComponent<CanvasScaler>(gameObject);
        _canvas.renderMode = RenderMode.ScreenSpaceCamera;
        _canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        _canvasScaler.referenceResolution = new Vector2(1920, 1080);
        _canvas.worldCamera = Camera.main;
    }

    protected virtual void Playinit()
    {

    }
}
