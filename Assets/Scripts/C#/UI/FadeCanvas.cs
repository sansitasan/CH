using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeCanvas : BaseCanvas
{
    [SerializeField]
    private Image _image;
    [SerializeField]
    private Material _m;
    private readonly int _materialMode = Shader.PropertyToID("_Mode");
    private readonly int _materialFade = Shader.PropertyToID("_Fade");

    public enum FadeMode
    {
        Base,
        Circle
    }

    protected override void EditAwakeInit()
    {
        base.EditAwakeInit();
        _image = Util.GetComponentInChild<Image>(transform, gameObject.activeSelf);
        _canvas.sortingLayerID = SortingLayer.NameToID("GUI");
        _canvas.sortingOrder = 20;
    }

    protected override void Playinit()
    {
        base.Playinit();
        //SceneManager.activeSceneChanged += SetCamera;
    }

    public async UniTask FadeOutScene(float time = 0.5f, FadeMode mode = FadeMode.Base)
    {
        _canvas.gameObject.SetActive(true);
        _image.gameObject.SetActive(true);
        float temp = time;
        if (mode == FadeMode.Base)
            _m.SetInt(_materialMode, 0);
        else
        {
            _m.SetInt(_materialMode, 1);
            Debug.Log(Mouse.current.position.x);
        }

        while (temp > 0)
        {
            await UniTask.DelayFrame(1);
            temp -= Time.deltaTime;
            _m.SetFloat(_materialFade, 1 - temp / time);
        }

    }

    public async UniTask FadeInScene(float time = 0.5f, FadeMode mode = FadeMode.Base)
    {
        float temp = time;
        if (mode == FadeMode.Base)
            _m.SetInt(_materialMode, 0);
        else
        {
            _m.SetInt(_materialMode, 1);
            Debug.Log(Mouse.current.position.y);
        }

        while (temp > 0)
        {
            _m.SetFloat(_materialFade, temp / time);
            await UniTask.DelayFrame(1);
            temp -= Time.deltaTime;
        }
        _image.gameObject.SetActive(false);
        _canvas.gameObject.SetActive(false);
    }

    private void SetCamera(Scene cur, Scene next)
    {
        if (GameScene.Instance == null)
            _canvas.worldCamera = Camera.main;
        else
        {
            _canvas.worldCamera = GameScene.Instance.UICam;
        }
    }
}
