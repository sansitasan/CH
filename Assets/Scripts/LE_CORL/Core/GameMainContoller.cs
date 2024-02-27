using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMainContoller : MonoBehaviour
{
    const string OVERRIDE_SCRIPT_SCENE_ID = "ScriptScene";
    const string OVERRIDE_SETTING_SCENE_ID = "SettingScene";

    public static GameMainContoller Instance { get; private set; }

    bool onScriptSceneOpen = false;
    bool onSettingSceneOpen = false;




    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        Destroy(gameObject);
    }

    public void LoadScriptsScene()
    {
        if(!onScriptSceneOpen)
        {
            SceneManager.LoadSceneAsync(OVERRIDE_SCRIPT_SCENE_ID, LoadSceneMode.Additive);
        }
        else
        {
            SceneManager.UnloadSceneAsync(OVERRIDE_SCRIPT_SCENE_ID);
        }
    }


    public void LoadSettingScene()
    {
        if(!onSettingSceneOpen)
        {
            SceneManager.LoadSceneAsync(OVERRIDE_SETTING_SCENE_ID, LoadSceneMode.Additive);
        }
        else
        {
            SceneManager.UnloadSceneAsync(OVERRIDE_SETTING_SCENE_ID);
        }
    }


}
