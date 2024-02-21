using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameScene : MonoBehaviour
{
    [SerializeField]
    protected PlayerModel _playerModel;
    [SerializeField]
    protected DialogPanel _dialogPanel;

    public int Stage;

    private void Awake()
    {
        Vector3 x = new Vector3(10.92f, 11.4f, 11);
        if (GameManager.Instance.BEdit)
            StartAsyncInEdit().Forget();
        else
            GameManager.Instance.ActiveScene += Init;
    }

    private void Init(SceneName prev, SceneName next)
    {
        GameManager.Instance.ActiveScene -= Init;
        StartAsync().Forget();
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
        _playerModel.Script(false);
    }

    public virtual void GetEvent(EventTypes type)
    {
        if (type != EventTypes.Middle)
        {
            _dialogPanel.StartScript(type);
            _playerModel.Script(true);
        }
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
