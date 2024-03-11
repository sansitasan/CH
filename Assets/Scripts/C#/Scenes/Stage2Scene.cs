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
                _playerModel.DisableInput(true);
            }

            else
            {
                //다른 이벤트
            }
        }
    }

    protected override async UniTask StartAsync()
    {
        _bd.Init(ResourceManager.Instance.GetScriptableObject(), _playerModel.transform.position + Vector3.left);
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
