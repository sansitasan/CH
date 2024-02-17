using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameScene : MonoBehaviour
{
    [SerializeField]
    private PlayerModel _playerModel;
    [SerializeField]
    private DialogPanel _dialogPanel;
    [SerializeField]
    private BD _bd;

    public int Stage;
    public int MaxCount;
    private int _count;

    private void Awake()
    {
        if (GameManager.Instance.BEdit)
            StartAsyncInEdit().Forget();
        else
            GameManager.Instance.ActiveScene += Init;
        MaxCount = transform.childCount - 1;
    }

    private void Init(SceneName prev, SceneName next)
    {
        GameManager.Instance.ActiveScene -= Init;
        StartAsync().Forget();
    }
    private async UniTask StartAsync()
    {
        if (_bd != null)
        {
            _bd.Init(ResourceManager.Instance.GetScriptableObject());
            _bd.AfterScriptInit().Forget();
        }
        _playerModel.Init(ResourceManager.Instance.GetScriptableObject());
        await _playerModel.AfterScriptInit();
        if (_bd != null)
        {
            _bd.enabled = true;
        }
        _playerModel.enabled = true;
        GetEvent(EventTypes.Start);
    }

    private async UniTask StartAsyncInEdit()
    {
        await TResourceManager.Instance.LoadAsyncAssets();
        _playerModel = FindObjectOfType<PlayerModel>();
        if (_bd != null)
        {
            _bd.Init(TResourceManager.Instance.GetScriptableObject(Stage));
            _bd.AfterScriptInit().Forget();
        }
        _playerModel.Init(TResourceManager.Instance.GetScriptableObject(Stage));
        await _playerModel.AfterScriptInit();
        if (_bd != null)
        {
            _bd.enabled = true;
        }
        _playerModel.enabled = true;
        _playerModel.Script(false);
    }

    public void GetEvent(EventTypes type)
    {
        if (type != EventTypes.Middle)
        {
            _dialogPanel.StartScript(type);
            _playerModel.Script(true);
        }

        else
        {
            ++_count;
            if (_count == MaxCount)
            {
                _dialogPanel.StartScript(EventTypes.End);
                _playerModel.Script(true);
            }

            else
            {
                //다른 이벤트
            }
        }
    }

    public void EndEvent()
    {
        _playerModel.Script(false);
    }

    private void Clear()
    {
        _playerModel.Dispose();
    }
}
