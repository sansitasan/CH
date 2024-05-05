using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class Stage3Scene : GameScene
{
    [SerializeField]
    private BD _bd;

    [SerializeField]
    private List<Vector2> _spawnPoints = new List<Vector2>();
    [SerializeField]
    private List<MapController> _mapControllers = new List<MapController>();

    public int MaxCount;
    public int Count { get; private set; }

    public override void GetEvent(EventTypes type)
    {
        base.GetEvent(type);

        if (type == EventTypes.Middle)
        {
            ++Count;
            if (Count == MaxCount)
            {
                GetEvent(EventTypes.End);
            }

            else
            {
                //다른 이벤트
                MoveCharacter();
            }
        }
    }

    public override void Restart()
    {
        base.Restart();
        //TODO: 저장할 때 몇 단계인지 저장한 후 단계를 가져오기
    }

    protected override async UniTask StartAsync()
    {
        char name = SceneManager.GetActiveScene().name[0];
        int stage = name - '0' - 1;
        LResourcesManager.TryGetStageData(stage, out var stageData);
        _bd.Init(stageData, _playerModel.transform.position + Vector3.left);
        _bd.AfterScriptInit().Forget();

        await base.StartAsync();

    }

    protected override async UniTask StartAsyncInEdit()
    {
        await TResourceManager.Instance.LoadAsyncAssets();

        _bd.Init(TResourceManager.Instance.GetScriptableObject(Stage), _playerModel.transform.position + Vector3.left);
        _bd.AfterScriptInit().Forget();

        await base.StartAsyncInEdit();
    }

    private void MoveCharacter()
    {
        _playerModel?.Dispose();
        _playerModel.transform.position = _spawnPoints[Count];

        int stage = Stage;
        if (!GameMainContoller.IsTest)
        {
            char name = SceneManager.GetActiveScene().name[0];
            stage = name - '0' - 1;
        }
        LResourcesManager.TryGetStageData(stage, out var stageData);
        _bd?.Init(stageData, _playerModel.transform.position + Vector3.left);

        _playerModel?.Init(stageData);
        _playerModel?.DisableInput(false);
    }

    public override void OnNotify(Playable origin, INotification notification, object context)
    {
        base.OnNotify(origin, notification, context);
        if (_playerModel != null)
        {
            MoveCharacter();
            _mapControllers[Count].Restart();
        }
    }
}
