using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Level4PlayerController : PlayerControllerBase
{
    const string ANIMATOR_KEY_IS_MOVE = "Move";
    const string ANIMATOR_KEY_Y = "Y";

    [Header("General")]
    [SerializeField] SpriteRenderer playerRenderer;
    [SerializeField] Animator playerAnimator;
    [SerializeField] float moveSpeed;

    [Header("Skill")]
    [SerializeField] float skillCooldown;
    [SerializeField] float skillDuration;
    [SerializeField] Color targetColor;

    bool onMove = false;
    Vector2 moveInput;
    float skillActedtime;

    public override PlayerSkillState SkillState 
    {
        get
        {
            if (Time.time >= skillActedtime + skillDuration + skillCooldown)
                return PlayerSkillState.Ready;
            else if (Time.time >= skillActedtime + skillDuration)
                return PlayerSkillState.OnCoolDown;
            else if (Time.time >= skillActedtime)
                return PlayerSkillState.OnActivating;
            else
                return PlayerSkillState.Ready;
        }
    }

    



    #region Mono
    private void Update()
    {
        playerAnimator.SetBool(ANIMATOR_KEY_IS_MOVE, onMove);
        int parseY = moveInput.y > 0 ? 1 : moveInput.y < 0 ? -1 : 0;
        playerAnimator.SetInteger(ANIMATOR_KEY_Y, parseY);
        playerRenderer.flipX = moveInput.x > 0 ? false : moveInput.x < 0 ? true : playerRenderer.flipX;
    }
    private void FixedUpdate()
    {
        if (onMove)
            transform.Translate(Time.deltaTime * moveSpeed * moveInput);
    }
    #endregion


    #region Overrides

    public override void Init()
    {
        base.Init();
        playerAnimator = playerRenderer.GetComponent<Animator>();
    }
    public override void Player_Move_performed(InputAction.CallbackContext context)
    {
        onMove = true;
        moveInput = context.ReadValue<Vector2>().normalized;
    }
    public override void Player_Move_canceled(InputAction.CallbackContext context)
    {
        onMove = false;
    }
    public override void Player_Action1_performed(InputAction.CallbackContext context)
    {
        return;
    }
    public override void Player_Action2_performed(InputAction.CallbackContext context)
    {
        ActiveSkill().Forget();
    }

    /*
    public override void UI_Click(InputAction.CallbackContext callbackContext)
    {
        throw new System.NotImplementedException();
    }

    public override void UI_Escape(InputAction.CallbackContext callbackContext)
    {
        throw new System.NotImplementedException();
    }

    public override void UI_Move_canceled(InputAction.CallbackContext callbackContext)
    {
        throw new System.NotImplementedException();
    }

    public override void UI_Move_performed(InputAction.CallbackContext callbackContext)
    {
        throw new System.NotImplementedException();
    }

    public override void UI_Point(InputAction.CallbackContext callbackContext)
    {
        throw new System.NotImplementedException();
    }

    public override void UI_Submit(InputAction.CallbackContext callbackContext)
    {
        throw new System.NotImplementedException();
    }
    */
    #endregion

    #region Skill

    async UniTaskVoid ActiveSkill()
    {
        if (SkillState != PlayerSkillState.Ready)
            return;

        // 스킬 사용 효과
        skillActedtime = Time.time;
        playerRenderer.color = targetColor;
        
        await UniTask.Delay(TimeSpan.FromSeconds(skillDuration));

        // 원상태 복구
        playerRenderer.color = Color.white;
    }

    #endregion
}
