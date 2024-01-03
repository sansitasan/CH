using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogPanel : MonoBehaviour
{
    private Image _image;
    private TextMeshProUGUI _name;
    private TextMeshProUGUI _dialog;
    private StringBuilder _dialogText;
    private Button _nextButton;

    private CancellationTokenSource _cts;
    private List<Script> _scripts;
    private int _cnt;
    private bool _btalk;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        _image = transform.GetChild(0).GetComponent<Image>();
        _name = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        _dialog = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        _nextButton = transform.GetChild(3).GetComponent<Button>();
        _cts = new CancellationTokenSource();
        _scripts = ResourceManager.Instance.TryGetScript("start1");
        _nextButton.onClick.AddListener(Click);
        _btalk = false;
        _dialogText = new StringBuilder();
    }

    private void Click()
    {
        CheckTalk().Forget();
    }

    private async UniTaskVoid CheckTalk()
    {
        if (_scripts.Count == _cnt)
        {
            SceneManager.UnloadSceneAsync("ScriptScene");
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

    private async UniTask SetTextAsync(Script script)
    {
        try
        {
            _name.text = script.talkname[0];
            _image.sprite = ResourceManager.Instance.GetSprite(script.data.nameNstate);
            int len = script.data.dialog.Length;
            _dialogText.Clear();
            _dialogText.Capacity = len;

            for (int i = 0; i < len; ++i)
            {
                if (script.data.dialog[i].Equals('<'))
                {
                    while (true)
                    {
                        _dialogText.Append(script.data.dialog[i]);
                        if (script.data.dialog[i].Equals('>'))
                            break;
                        ++i;
                    }
                }

                else
                    _dialogText.Append(script.data.dialog[i]);
                _dialog.text = _dialogText.ToString();
                await UniTask.DelayFrame(1, cancellationToken: _cts.Token);
            }
        }

        catch
        {
            _dialog.text = script.data.dialog;
            _cts.Dispose();
            _cts = new CancellationTokenSource();
        }
    }

    private void OnDestroy()
    {
        _cts.Cancel();
        _cts.Dispose();
        _cts = null;
    }
}
