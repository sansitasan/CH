using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartScene : MonoBehaviour
{
    [SerializeField]
    private Button _startButton;
    [SerializeField]
    private Button _testButton;

    private void Awake()
    {
        Init();
        ResourceManager.Instance.Init();
    }

    private void Init()
    {
        _startButton.onClick.AddListener(ChangeScene);
        _testButton.onClick.AddListener(TestAddScene);
    }

    private void ChangeScene()
    {
        GameManager.Instance.SceneChangeAsync(SceneName.StartScene, SceneName.LevelScene, FadeCanvas.FadeMode.Circle).Forget();
    }

    private void TestAddScene()
    {
        SceneManager.LoadScene("ScriptScene", LoadSceneMode.Additive);
    }
}
