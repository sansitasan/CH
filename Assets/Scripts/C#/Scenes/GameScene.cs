using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;

public class GameScene : MonoBehaviour, IDisposable, INotificationReceiver
{
    [SerializeField]
    protected PlayerModel _playerModel;
    protected PlayableDirector _pd;
    protected Dictionary<EventTypes, List<TimelineAsset>> _taDict = new Dictionary<EventTypes, List<TimelineAsset>>();

    [SerializeField]
    private List<TimelineAsset> _taList = new List<TimelineAsset>();
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

        if (type == EventTypes.Start || type == EventTypes.End)
        {
            int tNum = (int)type;
            GameMainContoller.Instance.LoadScriptsScene((ScriptScene.ScriptEventType)tNum);
            _playerModel.DisableInput(true);
        }

        else if (type == EventTypes.Dead)
        {
            _playerModel.DisableInput(true);
            MainCamera.SetMask(9);
            _pd.Play(_taDict[type][0]);
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
        _pd = GetComponent<PlayableDirector>();
        _taDict.Add(EventTypes.Dead, _taList);
    }

    private void Init()
    {
        _instance = this;
        _pd = GetComponent<PlayableDirector>();
        _taDict.Add(EventTypes.Dead, _taList);

        GameMainContoller.Instance.ActiveScene -= Init;
        GameMainContoller.Instance.ChangeScene += Restart;
        StartAsync().Forget();
    }

    public virtual void OnNotify(Playable origin, INotification notification, object context)
    {
        if ((EventTypes)notification.id.GetHashCode() == EventTypes.Dead)
            MainCamera.SetMask(9);
        _playerModel.DisableInput(false);
    }
}
