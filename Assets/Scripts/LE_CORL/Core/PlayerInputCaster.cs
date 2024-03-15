using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerInputCaster : MonoBehaviour, ICore
{
    PlayerInput input = null;
    public bool IsIntitalized => throw new System.NotImplementedException();



    public void Init()
    {
        input = GetComponent<PlayerInput>();
        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }
    public void Disable()
    {
        throw new NotImplementedException();
    }

    private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
    {

    }
}