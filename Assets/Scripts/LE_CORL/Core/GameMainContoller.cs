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

[DefaultExecutionOrder(-1)]
public class GameMainContoller : MonoBehaviour
{
    const string BASE_LOBBY_SCENE_ID = "Lobby";
    const string BASE_STAGE_SCENE_ID = "Stage";
    const string OVERRIDE_LOADING_SCENE_ID = "Loading";
    const string OVERRIDE_SCRIPT_SCENE_ID = "ScriptScene";
    const string OVERRIDE_SETTING_SCENE_ID = "Menu";


    public static GameMainContoller Instance { get; private set; }
    public static bool IsIntitalized { get; private set; }
    public static bool IsTest { get; private set; }
    static Dictionary<Type, ICore> cores;

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
    bool onPause;

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
        var current = SceneManager.GetActiveScene().buildIndex;
        IsTest = current != 0;

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

        SceneManager.LoadScene(OVERRIDE_LOADING_SCENE_ID, LoadSceneMode.Additive);

        await UniTask.WaitUntil(() => !LoadingSceneController.OnAnimation);

        var targetSceneLoader = SceneManager.LoadSceneAsync(targetSceneIDX, LoadSceneMode.Additive);
        targetSceneLoader.allowSceneActivation = false;

        await UniTask.WhenAll(
            UniTask.WaitUntil(() => targetSceneLoader.progress >= 0.89),
            UniTask.Delay(TimeSpan.FromSeconds(1.5f))
            );

        targetSceneLoader.allowSceneActivation = true;
        Scene targetScene = SceneManager.GetSceneByBuildIndex(targetSceneIDX);
        await UniTask.WaitUntil(() => SceneManager.SetActiveScene(targetScene));

        print("Main - active scene changed");

        await UniTask.WhenAll(
            SceneManager.UnloadSceneAsync(lastScene).ToUniTask(), 
            UniTask.WaitUntil(() => !LoadingSceneController.OnAnimation)
            );
        await SceneManager.UnloadSceneAsync(OVERRIDE_LOADING_SCENE_ID).ToUniTask();
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


    public bool GamePause()
    {
        if (!onPause)
        {
            SceneManager.LoadSceneAsync(OVERRIDE_SETTING_SCENE_ID, LoadSceneMode.Additive);
            onPause = true;
        }
        else
        {
            SceneManager.UnloadSceneAsync(OVERRIDE_SETTING_SCENE_ID);
            onPause = false;
        }
        Time.timeScale = onPause ? 0.0f : 1.0f;
        return onPause;
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

    private void OnDisable()
    {
        foreach (var item in cores)
        {
            item.Value.Disable();
        }
    }
}

/// 24.03.23 
///     코어 업댓 2
/// 
///     Assets/Prefabs/Core/
///         --- Main Controller --- 프리팹
///         
/// 코어 오브젝트 관련
///     코어 오브젝트 하위에 EventSystem 배치,
///     코어 오브젝트 하위에 AudioListener 배치
/// 
/// 코어 스크립트 관련
///     싱글턴 저장 방식 일부 변경
///     외부에서 접근 방식 일부 변경
///     
///     
/// 씬 로드 관련
///     씬 호출 순서 변경
///         기존 | 메서드 호출 -> 로딩 씬 로드, 타겟 씬 로드 -> 로딩씬 언로드
///         변경 | 메서드 호출 -> 로딩 씬 로드 -> 타겟 씬 로드 -> 로딩씬 언로드
///     
/// 인풋 관련
///         오브젝트: 코어 프리팹 하위Player Input 
///         스크립트: Player Input Caster 
///         
///         플레이어 컴포넌트: PlayerControllerBase
///         
///         
///     