using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class TestScript : MonoBehaviour
{
    public enum TSStates
    {
        None,
        Loading,
        LoadComplete,
        SelectComplete,
        Complete
    }

    [Header("건드리지 말 것")]
    [SerializeField, ReadOnly(false)]
    private TResourceManager _resourceManager;

    [field: SerializeField, Header("현재 상태")]
    public TSStates State { get; private set; }
    [Header("현재 스크립트"), ReadOnly(false)]
    [SerializeField]
    private string _scriptName;

    [Header("스피드 조절")]
    [SerializeField, Range(1, 120)]
    private int _speed = 1;

    private Image _image;
    private TextMeshProUGUI _name;
    private TextMeshProUGUI _dialog;
    private StringBuilder _dialogText;

    private CancellationTokenSource _cts;
    private List<Script> _scripts;
    private int _cnt;
    private bool _btalk;

    public void InitTest()
    {
        Application.targetFrameRate = 60;
        _resourceManager = TResourceManager.Instance;
        Init();
        InitTR().Forget();
    }

    private void CheckSafety()
    {
        if (_cts == null)
            _cts = new CancellationTokenSource();
        if (_dialogText == null)
            _dialogText = new StringBuilder();
    }

    private void Init()
    {
        _image = transform.GetChild(0).GetComponent<Image>();
        _name = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        _dialog = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        _name.text = string.Empty;
        _dialog.text = string.Empty;

        _cts = new CancellationTokenSource();
        _btalk = false;
        _dialogText = new StringBuilder();
    }

    private async UniTask InitTR()
    {
        CheckSafety();
        State = TSStates.Loading;
        await TResourceManager.Instance.LoadAsyncAssets();
        State = TSStates.LoadComplete;
    }

    public void GetScript(string name)
    {
        _scriptName = name;
        _scripts = TResourceManager.Instance.TryGetScript(name);
        State = TSStates.SelectComplete;
        Click();
    }

    public void Click()
    {
        CheckSafety();
        CheckTalk().Forget();
    }

    private async UniTask CheckTalk()
    {
        if (_scripts.Count - 1 <= _cnt)
        {
            State = TSStates.Complete;
            return;
        }

        if (_btalk)
        {
            _cts.Cancel();
            _btalk = false;
        }

        else
        {
            _btalk = true;
            await SetTextAsync(_scripts[_cnt++]);
            _btalk = false;
        }
    }

    public void Clear()
    {
        CheckSafety();
        _cts.Cancel();
        _cts.Dispose();
        _cnt = 0;
        Init();
        State = TSStates.None;
    }

    private async UniTask SetTextAsync(Script script)
    {
        try
        {
            _name.text = script.talkname[0];
            _image.sprite = TResourceManager.Instance.GetSprite(script.character, script.emotion);
            int len = script.dialog.Length;

            if (_dialogText == null)
            {
                _dialogText = new StringBuilder();
            }

            _dialogText.Clear();
            _dialogText.Capacity = len;

            for (int i = 0; i < len; ++i)
            {
                if (script.dialog[i].Equals('<'))
                {
                    while (true)
                    {
                        _dialogText.Append(script.dialog[i]);
                        if (script.dialog[i].Equals('>'))
                            break;
                        ++i;
                    }
                }

                else
                    _dialogText.Append(script.dialog[i]);
                _dialog.text = _dialogText.ToString();
                await UniTask.DelayFrame(1 * _speed, cancellationToken: _cts.Token);
            }
        }

        catch
        {
            _dialog.text = script.dialog;
            _cts.Dispose();
            _cts = new CancellationTokenSource();
        }
    }
}
