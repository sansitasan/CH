using Assets.Scripts.LE_CORL.Core;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputCaster : MonoBehaviour, ICore
{
    public bool IsIntitalized => isInited;
    bool isInited = false;

    PlayerInputAction.UIActions uiInputMap;
    PlayerInputAction.Player_IsometricActions playerInputMap;

    PlayerControllerBase currentController;



    public void Init()
    {
        // 생성
        var action = new PlayerInputAction();
        action.Enable();
        uiInputMap = action.UI;
        playerInputMap = action.Player_Isometric;
        // 이벤트 등록
        /*
        // UI
        uiActionMap.Move.performed += UI_Move_performed;
        uiActionMap.Move.canceled += UI_Move_canceled;
        uiActionMap.Click.performed += UI_Click_performed;
        uiActionMap.Submit.performed += UI_Submit_performed;
        uiActionMap.Point.performed += UI_Point_performed;
        */
        uiInputMap.Escape.performed += Pause_performed;

        // Player
        playerInputMap.Move.performed += Player_Move_performed;
        playerInputMap.Move.canceled += Player_Move_canceled;
        playerInputMap.Action1.performed += Player_Action1_performed;
        playerInputMap.Action1.canceled += Player_Action1_canceled;
        playerInputMap.Action2.performed += Player_Action2_performed;
        playerInputMap.Action2.canceled += Player_Alt_Interaction_canceled;
        playerInputMap.Pause.performed += Pause_performed;

        SwitchInputMap(true);

        isInited = true;
    }


    #region Player
    private void Player_Move_performed(InputAction.CallbackContext obj)
        => currentController?.Player_Move_performed(obj);
    private void Player_Move_canceled(InputAction.CallbackContext obj)
        => currentController?.Player_Move_canceled(obj);
    private void Player_Action1_performed(InputAction.CallbackContext obj)
        => currentController?.Player_Action1_performed(obj);
    private void Player_Action1_canceled(InputAction.CallbackContext obj)
        => currentController?.Player_Action1_canceled(obj);
    private void Player_Action2_performed(InputAction.CallbackContext obj)
        => currentController?.Player_Action2_performed(obj);
    private void Player_Alt_Interaction_canceled(InputAction.CallbackContext obj)
        => currentController?.Player_Action2_canceled(obj);


    private void Pause_performed(InputAction.CallbackContext obj)
    {
        var isPause = GameMainContoller.Instance.GamePause();
        SwitchInputMap(isPause);
    }
    #endregion


    public void Disable()
    {
        // UI
        uiInputMap.Escape.performed -= Pause_performed;
        uiInputMap.Disable();   

        // Player
        playerInputMap.Move.performed -= Player_Move_performed;
        playerInputMap.Move.canceled -= Player_Move_canceled;
        playerInputMap.Action1.performed -= Player_Action1_performed;
        playerInputMap.Action1.canceled -= Player_Action1_canceled;
        playerInputMap.Action2.performed -= Player_Action2_performed;
        playerInputMap.Action2.canceled -= Player_Alt_Interaction_canceled;
        playerInputMap.Pause.performed -= Pause_performed;
        playerInputMap.Disable();

        currentController = null;
    }

    void SwitchInputMap(bool toUI)
    {
        if (toUI)
        {
            playerInputMap.Disable();
            uiInputMap.Enable();
        }
        else
        {
            uiInputMap.Disable();
            playerInputMap.Enable();
        }
    }

    public static void SwitchPlayerController(PlayerControllerBase playerControl)
    {
        var self = GameMainContoller.GetCore<PlayerInputCaster>();
        self.currentController = playerControl;
        self.SwitchInputMap(self.currentController == null);
    }
}