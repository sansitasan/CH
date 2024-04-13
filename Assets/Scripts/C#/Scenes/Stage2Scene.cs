using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                _dialogPanel.StartScript(EventTypes.End);
                _playerModel.Script(true);
            }

            else
            {
                //다른 이벤트
            }
        }
    }

    protected override async UniTask StartAsync()
    {
<<<<<<< HEAD
        _bd.Init(ResourceManager.Instance.GetScriptableObject());
=======
        char name = SceneManager.GetActiveScene().name[0];
        int stage = name - '0' - 1;
        LResourcesManager.TryGetStageData(stage, out var stageData);
        _bd.Init(stageData, _playerModel.transform.position + Vector3.left);
>>>>>>> f603817 (Divide UICamera, Fix error - Load Resource InComplete and Resource Dictionary Clear with Change Scene)
        _bd.AfterScriptInit().Forget();

        await base.StartAsync();

        _bd.enabled = true;
    }

    protected override async UniTask StartAsyncInEdit()
    {
        await TResourceManager.Instance.LoadAsyncAssets();
        if (_bd != null)
        {
            _bd.Init(TResourceManager.Instance.GetScriptableObject(Stage));
            _bd.AfterScriptInit().Forget();
        }
        await base.StartAsyncInEdit();

        if (_bd != null)
        {
            _bd.enabled = true;
        }
    }
}
