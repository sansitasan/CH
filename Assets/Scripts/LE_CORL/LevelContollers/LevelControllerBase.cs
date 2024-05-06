using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// 코어에서 접근하여 사용
/// 새로운 씬이 로드됐을 때, IsSceneReady가 아니라면 해당 씬에 원래 존재하던 ILevelSceneController 를 찾아 InitScene을 실행.
/// 만약 
/// </summary>
public interface ILevelSceneControllerBase
{
    public static bool IsSceneReady { get; private set; }

    public void InitScene();

    public void DiposeScene();
}

public abstract class LevelControllerBase : MonoBehaviour
{
    [Header("LevelControllerBase")]
    [SerializeField] protected int myIDX;


    private void Awake()
    {
        GameMainContoller.OnSceneLoadingOpStarted += GameMainContoller_OnSceneLoadingOpStarted;
        GameMainContoller.OnSceneLoadingOpComplet += GameMainContoller_OnSceneLoadingOpComplet;
    }

    private void GameMainContoller_OnSceneLoadingOpStarted(object sender, GameMainContoller.SceneChangeEventArgs e)
    {
        GameMainContoller.OnSceneLoadingOpStarted -= GameMainContoller_OnSceneLoadingOpStarted;

        if (e.to == GameMainContoller.LOBBY_SCENE_IDX)
            ToLobbyScene();
        DiposeScene();
    }

    private void GameMainContoller_OnSceneLoadingOpComplet(object sender, GameMainContoller.SceneChangeEventArgs e)
    {
        GameMainContoller.OnSceneLoadingOpComplet -= GameMainContoller_OnSceneLoadingOpComplet;

        if (e.from == GameMainContoller.LOBBY_SCENE_IDX)
            InitializeScene();
        StartSceneWithoutInit();
    }

    /// <summary>
    /// 씬 최초 실행시 실행
    /// </summary>
    protected abstract void InitializeScene();

    /// <summary>
    /// 씬 재시작시 실행
    /// </summary>
    protected abstract void StartSceneWithoutInit();

    /// <summary>
    /// 씬이 종료될 때 실행. Mono OnDisable() 
    /// </summary>
    protected abstract void DiposeScene();

    /// <summary>
    /// 로비 씬으로 갈때 실행
    /// </summary>
    protected abstract void ToLobbyScene();




}
