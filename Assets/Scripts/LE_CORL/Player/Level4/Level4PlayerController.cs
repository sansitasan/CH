using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Assets.Scripts.LE_CORL.Player.PlayerSkillBase;

public class Level4PlayerController : PlayerControllerBase
{
    const string ANIMATOR_KEY_IS_MOVE = "Move";
    const string ANIMATOR_KEY_Y = "Y";

    [Header("General")]
    [SerializeField] Animator playerAnimator;
    SpriteRenderer playerAnimatorSR;
    [SerializeField] SpriteRenderer skillStateSR;
    [SerializeField] float moveSpeed;

    [Header("Skill")]
    [SerializeField] float skillPreDelay;
    [SerializeField] float skillCooldown;
    [SerializeField] float skillDuration;
    [SerializeField] Sprite[] up;
    [SerializeField] Sprite[] down;
    [SerializeField] Sprite[] left;
    [SerializeField] Sprite[] right;
    [SerializeField] Color skillStateColor;


    bool onMove = false;
    public Vector2 MoveInput { get; private set; }
    float skillActedtime;

    public override PlayerSkillState SkillState => m_SkillState;

    PlayerSkillState m_SkillState;





    #region Mono
    private void Update()
    {
        playerAnimator.SetBool(ANIMATOR_KEY_IS_MOVE, onMove);
        int parseY = MoveInput.y > 0 ? 1 : MoveInput.y < 0 ? -1 : 0;
        playerAnimator.SetInteger(ANIMATOR_KEY_Y, parseY);
        playerAnimatorSR.flipX = MoveInput.x > 0 ? false : MoveInput.x < 0 ? true : playerAnimatorSR.flipX;
    }
    private void FixedUpdate()
    {
        if (onMove)
            transform.Translate(Time.deltaTime * moveSpeed * MoveInput);
    }
    #endregion


    #region Overrides

    public override void Init() 
    { 
        base.Init();
        playerAnimatorSR = playerAnimator.GetComponent<SpriteRenderer>();
        skillStateSR.gameObject.SetActive(false);
    }

    public override void Player_Move_performed(InputAction.CallbackContext context)
    {
        onMove = true;
        MoveInput = context.ReadValue<Vector2>().normalized;
    }
    public override void Player_Move_canceled(InputAction.CallbackContext context)
    {
        onMove = false;
    }
    public override void Player_Action1_performed(InputAction.CallbackContext context)
    {
        ActiveSkill().Forget();

    }
    public override void Player_Action2_performed(InputAction.CallbackContext context)
    {
        return;
    }
    #endregion

    #region Skill

    async UniTaskVoid ActiveSkill()
    {
        if (SkillState != PlayerSkillState.Ready)
            return;
        
        m_SkillState = PlayerSkillState.OnActivating;

        // 스킬 상태 렌더러로 전환
        playerAnimatorSR.gameObject.SetActive(false);
        skillStateSR.sprite = GetSpriteBySkillState(0);
        skillStateSR.gameObject.SetActive(true);

        float t = 0;
        while(t < 1)
        {
            skillStateSR.sprite = GetSpriteBySkillState(t);

            await UniTask.Yield();
            t += Time.deltaTime / skillPreDelay;
        }
        playerAnimatorSR.color = skillStateColor;
        playerAnimatorSR.gameObject.SetActive(true);
        skillStateSR.gameObject.SetActive(false);

        await UniTask.Delay(TimeSpan.FromSeconds(skillDuration));

        playerAnimatorSR.color = Color.white;
        m_SkillState = PlayerSkillState.OnCoolDown;

        await UniTask.Delay(TimeSpan.FromSeconds(skillCooldown));

        m_SkillState = PlayerSkillState.Ready;
    }

    Sprite GetSpriteBySkillState(float amount)
    {
        amount = Mathf.Clamp01(amount);
        int idx = Mathf.RoundToInt(Mathf.Lerp(0, up.Length - 1, amount)) ;
        if(MoveInput.y != 0)
        {
            return MoveInput.y > 0 ? up[idx] : down[idx];
        }
        else
        {
            return MoveInput.x > 0 ? right[idx] : left[idx];
        }
    }

    #endregion
}
