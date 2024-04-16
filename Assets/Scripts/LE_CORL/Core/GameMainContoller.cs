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
    public const int LOBBY_SCENE_IDX = 1;
    const string BASE_LOBBY_SCENE_ID = "Lobby";
    const string BASE_STAGE_SCENE_ID = "Stage";
    const string OVERRIDE_SCRIPT_SCENE_ID = "ScriptScene";
    const string OVERRIDE_SETTING_SCENE_ID = "Menu";

    private FadeCanvas fade;
    public static GameMainContoller Instance { get; private set; }
    public static bool IsIntitalized { get; private set; }
    public static bool IsTest { get; private set; }
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

    bool onLoading = false;
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
        fade = GetComponentInChildren<FadeCanvas>(true);
        fade.gameObject.SetActive(true);
        fade.gameObject.SetActive(false);
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

    public void LoadScene(int targetScene, FadeCanvas.FadeMode mode = FadeCanvas.FadeMode.Base) => ChangeActiveScene(targetScene, mode).Forget();
    public void ReloadScene(FadeCanvas.FadeMode mode = FadeCanvas.FadeMode.Base)
    {
        ChangeActiveScene(SceneManager.GetActiveScene().buildIndex, mode).Forget();
        
    }
    public void LoadLobby(FadeCanvas.FadeMode mode = FadeCanvas.FadeMode.Base) => ChangeActiveScene(1, mode).Forget();

    async UniTaskVoid ChangeActiveScene(int targetSceneIDX, FadeCanvas.FadeMode mode)
    {
        if (onPause)
            GamePause();
        if (onScriptSceneOpen)
            LoadScriptsScene();

        print("Main - ChangeActiveScene");
        Scene lastScene = SceneManager.GetActiveScene();
        onLoading = true;

        await fade.FadeOutScene(0.5f, mode);

        AsyncOperation targetSceneLoader = SceneManager.LoadSceneAsync(targetSceneIDX, LoadSceneMode.Additive);
        targetSceneLoader.allowSceneActivation = false;

        await UniTask.WhenAll(
            UniTask.WaitUntil(() => targetSceneLoader.progress >= 0.89),
            UniTask.Delay(TimeSpan.FromSeconds(1.5f))
            );

        targetSceneLoader.allowSceneActivation = true;
        await UniTask.WaitUntil(() => targetSceneLoader.isDone);

        await UniTask.WhenAll(
            SceneManager.UnloadSceneAsync(lastScene).ToUniTask(),
            fade.FadeInScene(0.5f, mode)
            );

        onLoading = false;
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


    public bool GamePause()
    {
        if (onLoading || SceneManager.GetActiveScene().buildIndex == 0)
            return false;

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
        Time.timeScale = onPause ? 0 : 1;
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
/// 24.04.14 fix bug -load scene : 코어 업뎃 3차
/// 씬 로드 관련
///     Active Scene 을 넘겨주는 로직 일부 변경
///     로딩 중 퍼즈가 불가능하게 변경
///     타이틀 씬에서 퍼즈가 불가능하게 변경
///     
///     게임 씬에서 "다시하기"를 선택했을 때, 중복 씬이 활성화되던 문제 해결
///     게임 씬에서 "다시하기"를 선택했을 때, 페이드 인 상태에서 로딩 진행이 멈추던 문제 해결
///     
///     