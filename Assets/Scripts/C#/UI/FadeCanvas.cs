using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeCanvas : BaseCanvas
{
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

    protected override void Init()
    {
        base.Init();
        _image = Util.GetOrAddComponent<Image>(transform.GetChild(0));
        DontDestroyOnLoad(gameObject);
        _canvas.sortingOrder = 100;
    }

    public async UniTask FadeOutScene(float time = 0.5f, FadeMode mode = FadeMode.Base)
    {
        _canvas.gameObject.SetActive(true);
        _image.gameObject.SetActive(true);
        float temp = time;
        if (mode == FadeMode.Base)
            _m.SetInt(_materialMode, 0);
        else
            _m.SetInt(_materialMode, 1);

        while (temp > 0)
        {
            await UniTask.DelayFrame(1);
            temp -= Time.deltaTime;
            _m.SetFloat(_materialFade, 1 - temp / time);
        }
        Debug.Log($"temp is {temp}, Fade is {_m.GetFloat(_materialFade)}");
    }

    public async UniTask FadeInScene(float time = 0.5f, FadeMode mode = FadeMode.Base)
    {
        float temp = time;
        if (mode == FadeMode.Base)
            _m.SetInt(_materialMode, 0);
        else
            _m.SetInt(_materialMode, 1);

        while (temp > 0)
        {
            _m.SetFloat(_materialFade, temp / time);
            await UniTask.DelayFrame(1);
            temp -= Time.deltaTime;
        }
        _image.gameObject.SetActive(false);
        _canvas.gameObject.SetActive(false);
    }
}
