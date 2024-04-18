using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stage2Scene : GameScene
{
    [SerializeField]
    private BD _bd;

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
                GetEvent(EventTypes.End);
            }

            else
            {
                //다른 이벤트
            }
        }
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
}
