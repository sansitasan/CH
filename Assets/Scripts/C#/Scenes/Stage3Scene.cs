using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stage3Scene : GameScene
{
    [SerializeField]
    private BD _bd;

    [SerializeField]
    private List<Vector2> _spawnPoints = new List<Vector2>();

    public int MaxCount;
    private int _count;

    public override void GetEvent(EventTypes type)
    {
        base.GetEvent(type);

        if (type == EventTypes.Middle)
        {
            ++_count;
            if (_count == MaxCount)
            {
                _dialogPanel.StartScript(EventTypes.End);
                _playerModel.DisableInput(true);
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
        GameManager.Instance.FadeInOutAsync(MoveCharacter).Forget();
    }

    protected override void AwakeInit()
    {
        base.AwakeInit();
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
        _playerModel.Dispose();
        _playerModel.transform.position = _spawnPoints[_count];
        _bd.SetPos(_spawnPoints[_count] - Vector2.right);
        if (GameManager.Instance.BEdit)
            _playerModel.Init(TResourceManager.Instance.GetScriptableObject(Stage));
        else
        {
            char name = SceneManager.GetActiveScene().name[0];
            int stage = name - '0' - 1;
            LResourcesManager.TryGetStageData(stage, out var stageData);
            _playerModel.Init(stageData);
        }
        _playerModel.DisableInput(false);
    }
}
