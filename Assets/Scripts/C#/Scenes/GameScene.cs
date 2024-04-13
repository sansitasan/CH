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

    public EventTypes CurrentEventType { get; protected set; } 

    public static GameScene Instance { get => _instance; }
    private static GameScene _instance;
    public Camera UICam;

    public int Stage;

    private void Awake()
    {
        AwakeInit();
        //if (GameManager.Instance.BEdit)
        //    StartAsyncInEdit().Forget();
        if (GameMainContoller.Instance != null)
            GameMainContoller.Instance.ActiveScene += Init;
        else
            StartAsyncInEdit().Forget();
    }

    public virtual void Restart()
    {

    }

    public virtual void GetEvent(EventTypes type)
    {
        CurrentEventType = type;
        if (type != EventTypes.Middle)
        {
            GameMainContoller.Instance.LoadScriptsScene();
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
        char name = SceneManager.GetActiveScene().name[0];
        int stage = name - '0' - 1;
        LResourcesManager.TryGetStageData(stage, out var stageData);
        _playerModel.Init(stageData);
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

    private void Init()
    {
        GameMainContoller.Instance.ActiveScene -= Init;
        StartAsync().Forget();
    }
}
