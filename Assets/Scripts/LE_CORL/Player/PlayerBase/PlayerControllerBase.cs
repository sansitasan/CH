using Assets.Scripts.LE_CORL.Core;
using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerControllerBase : MonoBehaviour
{
    public enum PlayerSkillState { Ready, OnActivating, OnCoolDown, }

    [Header("Camera")]
    [SerializeField] protected CinemachineVirtualCamera vCam;
    [SerializeField] protected float camFov;

    public abstract PlayerSkillState SkillState { get; }

    private void OnEnable() => Init();
    private void OnDisable() => Disable();

    public virtual void Init() 
    {
        PlayerInputCaster.SwitchPlayerController(this);
        try
        {
            vCam.m_Lens.OrthographicSize = camFov;
        }
        catch (NullReferenceException e)
        {
            Debug.LogError(e);
            vCam = GetComponentInChildren<CinemachineVirtualCamera>();
            vCam.m_Lens.OrthographicSize = camFov;
        }

    }
    public virtual void Disable() { }

    /*
    public abstract void UI_Move_performed(InputAction.CallbackContext callbackContext);
    public abstract void UI_Move_canceled(InputAction.CallbackContext callbackContext);
    public abstract void UI_Point(InputAction.CallbackContext callbackContext) ;
    public abstract void UI_Click(InputAction.CallbackContext callbackContext) ;
    public abstract void UI_Submit(InputAction.CallbackContext callbackContext) ;
    public abstract void UI_Escape(InputAction.CallbackContext callbackContext) ;
    */


    public abstract void Player_Move_performed(InputAction.CallbackContext context) ;
    public abstract void Player_Move_canceled(InputAction.CallbackContext context) ;
    public abstract void Player_Action1_performed(InputAction.CallbackContext context) ;
    public virtual void Player_Action1_canceled(InputAction.CallbackContext context) { }
    public abstract void Player_Action2_performed(InputAction.CallbackContext context) ;
    public virtual void Player_Action2_canceled(InputAction.CallbackContext context) { }

}
