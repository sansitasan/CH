using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Assets.Scripts.LE_CORL.Player.PlayerSkillBase;

public class Level4PlayerController : PlayerControllerBase
{
    Vector2 startPosition = new Vector2(-1.85f, 8.88f);

    const string ANIMATOR_KEY_IS_MOVE = "OnMove"; // bool
    const string ANIMATOR_KEY_DEAD_TRIGGER = "Dead"; // trigger
    const string ANIMATOR_KEY_DEAD = "OnDead"; // trigger
    const string ANIMATOR_KEY_INPUT_X = "InputX"; // float
    const string ANIMATOR_KEY_INPUT_Y = "InputY"; // float
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

    [Header("Extra")]
    [SerializeField] float untouchableDuration;
    [SerializeField, Range(0, 1)] float untouchableColorAlpha;


    bool onMove = false;
    public Vector2 MoveInput { get; private set; }

    public override PlayerSkillState SkillState => m_SkillState;
    public bool IsUntouchable { get; private set; }

    PlayerSkillState m_SkillState;


    #region Mono
    private void Update()
    {
        playerAnimator.SetBool(ANIMATOR_KEY_IS_MOVE, onMove);

        int x = MoveInput.x > 0 ? 1 : MoveInput.x < 0 ? -1 : 0;
        int y = MoveInput.y > 0 ? 1 : MoveInput.y < 0 ? -1 : 0;
        playerAnimator.SetInteger(ANIMATOR_KEY_INPUT_X, x);
        playerAnimator.SetInteger(ANIMATOR_KEY_INPUT_Y, y);
        playerAnimatorSR.flipX = x > 0 ? false : x < 0 ? true : playerAnimatorSR.flipX;

        /*
        int parseY = MoveInput.y > 0 ? 1 : MoveInput.y < 0 ? -1 : 0;
        playerAnimator.SetInteger(ANIMATOR_KEY_Y, parseY);
        */
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
        MoveInput = context.ReadValue<Vector2>();
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


    /// <summary>
    /// 대미지가 들어갔으면 true, 들어가지 않았으면  false를 반환
    /// </summary>
    /// <returns></returns>
    public int PlayerDamaged(int currentHP)
    {
        if(IsUntouchable || SkillState == PlayerSkillState.OnActivating)
        {
            return currentHP;
        }
        currentHP--;

        if (currentHP <= 0)
        {
            playerAnimator.SetTrigger(ANIMATOR_KEY_DEAD_TRIGGER);
            playerAnimator.SetBool(ANIMATOR_KEY_DEAD, true);
        }
        else
            ToPlayerUntouchable().Forget();
        return currentHP;
    }

    public void SetPlayer(int hp)
    {

    }

    async UniTask ToPlayerUntouchable()
    {
        float start = Time.time;
        IsUntouchable = true; 
        while (Time.time <= start + untouchableDuration)
        {
            float alpha = Mathf.PingPong(Time.time, untouchableColorAlpha);
            playerAnimatorSR.color = new Color(1, 1, 1, alpha);
            await UniTask.Yield();
        }
        playerAnimatorSR.color = Color.white;
        IsUntouchable = false;
    }
}
