using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Obsolete]
public class StartScene : MonoBehaviour
{
    [SerializeField]
    private Button _startButton;

    private void Awake()
    {
        Init();
        ResourceManager.Instance.Init();
    }

    private void Init()
    {
        _startButton.onClick.AddListener(ChangeScene);
    }

    private void ChangeScene()
    {
        GameManager.Instance.SceneChangeAsync(SceneName.StartScene, SceneName.LevelScene, FadeCanvas.FadeMode.Circle).Forget();
    }
}
