using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScriptScene : MonoBehaviour
{
    private Image _image;
    private Sprite _temp;
    private TextMeshProUGUI _name;
    private TextMeshProUGUI _dialog;
    private StringBuilder _dialogText;
    private Button _nextButton;

    private CancellationTokenSource _cts;
    private List<Script> _scripts;

    [SerializeField, Header("스크립트 수")]
    private int _maxCnt;
    [SerializeField, Header("스크립트 건너뛰기")]
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
        _nextButton.onClick.AddListener(Click);
        _btalk = false;
        _dialogText = new StringBuilder();

        StartScript();
    }

    public void StartScript()
    {
        char name = SceneManager.GetActiveScene().name[0];
        int stage = name - '0' - 1;

        string targetName = $"{GameScene.Instance.CurrentEventType}{stage}";

        if (!LResourcesManager.TryGetScriptData(targetName, out _scripts))
        {
            Debug.LogError("cannot find script!");
        }
        gameObject.SetActive(true);
        _maxCnt = _scripts.Count;
        Click();
    }

    private void Click()
    {
        CheckTalk().Forget();
    }

    private async UniTask CheckTalk()
    {
        if (_scripts.Count == _cnt)
        {
            GameScene.Instance.EndEvent();
            if (GameScene.Instance != null)
                GameMainContoller.Instance.LoadScriptsScene();
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
            LResourcesManager.TryGetSprite(script.character, script.emotion, out _temp);
            if (_temp != null)
            {
                _image.sprite = _temp;
                _image.color = Color.white;
            }
            else
                _image.color = Color.clear;
            int len = script.dialog.Length;
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
                await UniTask.DelayFrame(1, cancellationToken: _cts.Token);
            }
        }

        catch
        {
            _dialog.text = script.dialog;
            if (_cnt < _scripts.Count)
            {
                _cts.Dispose();
                _cts = new CancellationTokenSource();
            }
        }
    }

    private void OnDestroy()
    {
        _cts.Cancel();
        _cts.Dispose();
        _cts = null;
    }
}
