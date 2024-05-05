using System.Collections;
using System.Collections.Generic;
using UnityEngine;



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

public class LevelControllerBase : MonoBehaviour
{
    public static  bool IsSceneReady { get; private set; }

    /// <summary>
    /// 씬 최초 실행시 동작. 씬 로드시 사전작업을 위한 함수
    /// </summary>
    protected virtual void InitializeScene()
    {
        IsSceneReady = true;
    }


    /// <summary>
    /// 씬 벗어날 때 동작. 로비로 돌아갈 때 현재 씬을 파기할 때 사용.
    /// </summary>
    protected virtual void DiposeScene()
    {
        IsSceneReady = false;
    }


}
