using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public interface ICore
{
    public bool IsIntitalized { get; }
    public void Init();
    public void Disable();
}

public class GameMainContoller : MonoBehaviour
{
    const string BASE_LOBBY_SCENE_ID = "Lobby";
    const string BASE_STAGE_SCENE_ID = "Stage";
    const string OVERRIDE_LOADING_SCENE_ID = "Loading";
    const string OVERRIDE_SCRIPT_SCENE_ID = "ScriptScene";
    const string OVERRIDE_SETTING_SCENE_ID = "Menu";


    public static GameMainContoller Instance { get; private set; }
    public static bool IsIntitalized { get; private set; }
    static Dictionary<Type, ICore> cores;
    public event Action ActiveScene;

    public static T GetCore<T>() where T : ICore
    {
        if (!cores.ContainsKey(typeof(T)))
        {
            Debug.LogError($"not registered content: {typeof(T)}");
            return default(T);
        }
        return (T)cores[typeof(T)];
    }

    bool onLoadingScene = false;
    bool onScriptSceneOpen = false;
    bool onMenuSceneOpen = false;



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitProject().Forget();

        }
        else
        {
            Destroy(gameObject);
        }
    }

    async UniTaskVoid InitProject()
    {
        cores = new Dictionary<Type, ICore>();

        var searched = GetComponentsInChildren<ICore>().ToList();
        foreach (var item in searched)
        {
            cores.Add(item.GetType(), item);
            item.Init();
        }
        await UniTask.WaitUntil(() => !searched.Any((ICore core) => !core.IsIntitalized));
        IsIntitalized = true;
        Debug.Log("Init Done");
    }

    public void LoadScene(int targetScene) => ChangeActiveScene(targetScene).Forget();

    async UniTaskVoid ChangeActiveScene(int targetSceneIDX)
    {
        print("Main - ChangeActiveScene");
        Scene lastScene = SceneManager.GetActiveScene();

        var targetSceneLoader = SceneManager.LoadSceneAsync(targetSceneIDX, LoadSceneMode.Additive);
        targetSceneLoader.allowSceneActivation = false;

        SceneManager.LoadScene(OVERRIDE_LOADING_SCENE_ID, LoadSceneMode.Additive);

        await UniTask.WhenAll(UniTask.WaitUntil(() => targetSceneLoader.progress >= 0.89), UniTask.Delay(TimeSpan.FromSeconds(2)));
        print("Main - target scene loaded");

        targetSceneLoader.allowSceneActivation = true;
        Scene targetScene = SceneManager.GetSceneByBuildIndex(targetSceneIDX);
        await UniTask.WaitUntil(() => SceneManager.SetActiveScene(targetScene));

        print("Main - active scene changed");

        await UniTask.WhenAll(
            SceneManager.UnloadSceneAsync(lastScene).ToUniTask(), 
            UniTask.WaitUntil(() => !LoadingSceneController.OnAnimation)
            );
        await SceneManager.UnloadSceneAsync(OVERRIDE_LOADING_SCENE_ID).ToUniTask();
        ActiveScene?.Invoke();
    }

    public void LoadScriptsScene()
    {
        if (!onScriptSceneOpen)
        {
            SceneManager.LoadSceneAsync(OVERRIDE_SCRIPT_SCENE_ID, LoadSceneMode.Additive);
            onScriptSceneOpen = true;
        }
        else
        {
            SceneManager.UnloadSceneAsync(OVERRIDE_SCRIPT_SCENE_ID);
            onScriptSceneOpen = false;
        }
    }


    public void LoadMenuScene()
    {
        if (!onMenuSceneOpen)
        {
            SceneManager.LoadSceneAsync(OVERRIDE_SETTING_SCENE_ID, LoadSceneMode.Additive);
            onMenuSceneOpen = true;
        }
        else
        {
            SceneManager.UnloadSceneAsync(OVERRIDE_SETTING_SCENE_ID);
            onMenuSceneOpen = false;
        }
    }

    [ContextMenu("Print Core List")]
    void Edit_PrintCoreList()
    {
        var searched = GetComponentsInChildren<ICore>().ToList();
        foreach (var item in searched)
        {
            print($"{item.GetType().ToString()} : {item}");
        }
    }

}

