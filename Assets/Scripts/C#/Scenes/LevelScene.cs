using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelScene : MonoBehaviour
{
    [SerializeField]
    private Button _startButton;
    [SerializeField] 
    private Button[] _levelButtons;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        _startButton.onClick.AddListener(() => ChangeScene(0));
        for (int i = 0; i < _levelButtons.Length; ++i)
        {
            int temp = i;
            _levelButtons[i].onClick.AddListener(() => ChangeScene(temp + 2));
        }
        GameManager.Instance.ChangeScene += Clear;
    }

    private void ChangeScene(int Scene)
    {
        GameManager.Instance.SceneChangeAsync(SceneName.LevelScene, (SceneName)Scene).Forget();
    }

    private void Clear(SceneName prev, SceneName next)
    {
        GameManager.Instance.ChangeScene -= Clear;
        _startButton.onClick.RemoveAllListeners();
        
        for (int i = 0; i < _levelButtons.Length; ++i)
            _levelButtons[i].onClick.RemoveAllListeners();
    }
}
