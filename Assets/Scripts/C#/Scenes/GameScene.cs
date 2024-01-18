using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameScene : MonoBehaviour
{
    private PlayerModel _playerModel;
    [SerializeField]
    private DialogPanel _dialogPanel;

    private void Awake()
    {
        GameManager.Instance.ActiveScene += Init;
    }

    private void Init(SceneName prev, SceneName next)
    {
        GameManager.Instance.ActiveScene -= Init;
        StartAsync().Forget();
    }
    private async UniTask StartAsync()
    {
        _playerModel = FindObjectOfType<PlayerModel>();
        _playerModel.Init();
        await _playerModel.AfterScriptInit();
        _playerModel.enabled = true;
        GetEvent(EventTypes.Start);
    }

    public void GetEvent(EventTypes type)
    {
        _dialogPanel.StartScript(type);
        _playerModel.Script(true);
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
