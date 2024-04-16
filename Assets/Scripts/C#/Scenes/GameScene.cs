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

    public EventTypes CurrentEventType { get; protected set; } 

    public static GameScene Instance { get => _instance; }
    private static GameScene _instance;

    public int Stage;

    private void Awake()
    {
        if (!GameMainContoller.IsTest)
            GameMainContoller.Instance.ActiveScene += Init;

        else
            StartAsyncInEdit().Forget();
    }

    public virtual void Restart()
    {
        _playerModel.Dispose();
        _instance = null;
        GameMainContoller.Instance.ChangeScene -= Restart;
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
        if (CurrentEventType == EventTypes.Start)
            _playerModel.DisableInput(false);

        else if (CurrentEventType == EventTypes.End)
        {
            Dispose();
        }

    }

    public void Dispose()
    {
        _playerModel.Dispose();
        _instance = null;
        GameMainContoller.Instance.ChangeScene -= Restart;

        //TODO Save
        GameMainContoller.Instance.LoadLobby();
    }

    protected virtual void AwakeInit()
    {
        _instance = this;
    }

    protected virtual async UniTask StartAsync()
    {
        bool bSkip = GameMainContoller.IsSkipScript;
        char name = SceneManager.GetActiveScene().name[0];
        int stage = name - '0' - 1;
        LResourcesManager.TryGetStageData(stage, out var stageData);
        _playerModel.Init(stageData);
        await _playerModel.AfterScriptInit();
        _playerModel.enabled = true;
        if (!bSkip)
            GetEvent(EventTypes.Start);
        else
            _playerModel.DisableInput(false);
    }

    protected virtual async UniTask StartAsyncInEdit()
    {
        _instance = this;
        GameMainContoller.Instance.ChangeScene += Restart;
        await TResourceManager.Instance.LoadAsyncAssets();
        _playerModel.Init(TResourceManager.Instance.GetScriptableObject(Stage));
        await _playerModel.AfterScriptInit();
        _playerModel.enabled = true;
        _playerModel.DisableInput(false);
    }

    private void Init()
    {
        _instance = this;
        GameMainContoller.Instance.ActiveScene -= Init;
        GameMainContoller.Instance.ChangeScene += Restart;
        StartAsync().Forget();
    }
}
