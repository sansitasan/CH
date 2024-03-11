using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameScene : MonoBehaviour, IDisposable
{
    [SerializeField]
    protected PlayerModel _playerModel;
    [SerializeField]
    protected DialogPanel _dialogPanel;

    public static GameScene Instance { get => _instance; }
    private static GameScene _instance;
    public Camera UICam;

    public int Stage;

    private void Awake()
    {
        AwakeInit();
        Vector3 x = new Vector3(10.92f, 11.4f, 11);
        if (GameManager.Instance.BEdit)
            StartAsyncInEdit().Forget();
        else
            GameManager.Instance.ActiveScene += Init;
    }

    public virtual void Restart()
    {

    }

    public virtual void GetEvent(EventTypes type)
    {
        if (type != EventTypes.Middle)
        {
            _dialogPanel.StartScript(type);
            _playerModel.DisableInput(true);
        }
    }

    public void EndEvent()
    {
        _playerModel.DisableInput(false);
    }

    public void Dispose()
    {
        _playerModel.Dispose();
        _instance = null;
    }

    protected virtual void AwakeInit()
    {
        _instance = this;
    }

    protected virtual async UniTask StartAsync()
    {
        _playerModel.Init(ResourceManager.Instance.GetScriptableObject());
        await _playerModel.AfterScriptInit();
        _playerModel.enabled = true;
        GetEvent(EventTypes.Start);
    }

    protected virtual async UniTask StartAsyncInEdit()
    {
        await TResourceManager.Instance.LoadAsyncAssets();
        _playerModel = FindObjectOfType<PlayerModel>();
        _playerModel.Init(TResourceManager.Instance.GetScriptableObject(Stage));
        await _playerModel.AfterScriptInit();
        _playerModel.enabled = true;
        _playerModel.DisableInput(false);
    }

    private void Init(SceneName prev, SceneName next)
    {
        GameManager.Instance.ActiveScene -= Init;
        StartAsync().Forget();
    }
}
