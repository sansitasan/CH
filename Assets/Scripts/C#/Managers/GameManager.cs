using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneName
{
    StartScene,
    LevelScene,
    Stage1Scene, 
    Stage2Scene,
    Stage3Scene,
    Stage4Scene,
    Stage5Scene,
    TempScene,
}

public struct Stage
{
    public short stage;
    public ushort[] successcnt;

    public Stage(short stage)
    {
        this.stage = stage;
        successcnt = new ushort[5] { 0, 0, 0, 0, 0 };
    }
}
[Obsolete]
public class GameManager 
{
    public static GameManager Instance
    {
        get
        {
            if (g_instance == null)
                g_instance = new GameManager();

            return g_instance;
        }
    }

    private static GameManager g_instance;

    public event Action<SceneName, SceneName> ChangeScene;
    public event Action<SceneName, SceneName> ActiveScene;
    public Stage Stage { get => _stage; }
    public int CurStage { get => _curStage; }
    private Stage _stage;
    private int _curStage;
    private FadeCanvas _fadeCanvas;
    private string _savepath;

    public readonly bool BEdit = false;

    private GameManager()
    {
        if (SceneManager.GetActiveScene().name != "0.StartScene")
        {
            BEdit = true;
        }
        if (!BEdit)
        {
            _savepath = Path.Combine(Application.persistentDataPath, "stage");
            _stage = LoadStage();
            _fadeCanvas = GameObject.Find("FadeCanvas").GetComponent<FadeCanvas>();
        }
        Application.targetFrameRate = 60;
    }

    public async UniTaskVoid SceneChangeAsync(SceneName prev, SceneName next, FadeCanvas.FadeMode mode = FadeCanvas.FadeMode.Base)
    {
        ChangeScene?.Invoke(prev, next);
        await _fadeCanvas.FadeOutScene(1, mode);
        SceneManager.LoadScene((int)SceneName.TempScene);
        await UniTask.WaitUntil(() => SceneManager.GetActiveScene().buildIndex == (int)SceneName.TempScene);

        AsyncOperation ao = SceneManager.LoadSceneAsync((int)next);
        ao.allowSceneActivation = false;
        GC.Collect();
        GC.WaitForPendingFinalizers();
        _curStage = (int)next - 1;
        await UniTask.WhenAll(UniTask.WaitUntil(() => ao.progress >= 0.89), UniTask.Delay(TimeSpan.FromSeconds(2)));
        ao.allowSceneActivation = true;
        await UniTask.WaitUntil(() => SceneManager.GetActiveScene().buildIndex == (int)next);
        await _fadeCanvas.FadeInScene(1, mode);

        ActiveScene?.Invoke(prev, next);
    }

    public async UniTask FadeInOutAsync(Action action, float time = 0.5f, FadeCanvas.FadeMode mode = FadeCanvas.FadeMode.Base)
    {
        await _fadeCanvas.FadeOutScene(time, mode);

        action?.Invoke();

        await _fadeCanvas.FadeInScene(time, mode);
    }

    public void SuccessSave(short stage)
    {
        if (_stage.stage < stage)
        {
            ++_stage.stage;
        }
        ++_stage.successcnt[stage - 1];
        string stagedata = JsonUtility.ToJson(Stage);
        File.WriteAllText(_savepath, stagedata);
    }

    private Stage LoadStage()
    {
        if (File.Exists(_savepath))
        {
            string voldata = File.ReadAllText(_savepath);
            Stage stage = JsonUtility.FromJson<Stage>(voldata);
            
            return stage;
        }

        else
            return new Stage(0);
    }
}
