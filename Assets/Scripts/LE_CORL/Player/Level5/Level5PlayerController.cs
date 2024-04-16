using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Level5PlayerController : PlayerControllerBase
{
    [Header("Player")]
    [SerializeField] LayerMask groundMask;
    [SerializeField] float jumpForce = 10;
    [SerializeField] float moveSpeed = 10;
    [SerializeField] bool onRun;

    [SerializeField] Level5HookSystem hookSystem;

    public override PlayerSkillState SkillState => m_skillState;
    PlayerSkillState m_skillState;
    BoxCollider2D m_BoxCollider2D;
    Rigidbody2D m_Rigidbody2D;

    bool onGround = false;
    bool onThrowing = false;
    bool onHooked = false;

    public static event EventHandler<PlayerStateChangeEventArgs> OnPlayStateChanged;
    public class PlayerStateChangeEventArgs : EventArgs
    {
        public bool onRunning;
        public float speed;
    }

    public override void Init()
    {
        base.Init();
        m_BoxCollider2D = GetComponent<BoxCollider2D>();
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }



    private void FixedUpdate()
    {
        // 땅 판정
        int mask = 1 << groundMask;
        Vector2 pos = (Vector2)transform.position + Vector2.down * m_BoxCollider2D.size / 2f;
        var tmp = Physics2D.Raycast(pos, Vector2.down, .1f, groundMask);
        onGround = (bool)tmp;

        if (onGround && onMoveInput)
        {
            m_Rigidbody2D.AddForce(Time.deltaTime * moveSpeed * moveInput, ForceMode2D.Force);
        }
    }

    private void Update()
    {
        if (!onRun) return;
        transform.Translate(Time.deltaTime * moveSpeed * Vector2.right);
    }


    void Jump()
    {
        m_Rigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    /// <summary>
    /// Hook Trigger를 던짐
    /// </summary>
    void ThrowHook() => hookSystem.ThrowHook();

    #region 입력

    public override void Player_Action1_performed(InputAction.CallbackContext context)
    {
        if(onGround)
        {
            Jump();
            return;
        }
        else
        {
            ThrowHook();
        }
    }
    public override void Player_Action1_canceled(InputAction.CallbackContext context)
    {
        hookSystem.CancelHook();
    }

    #endregion




    #region 사용하지 않는 입력키

    bool onMoveInput;
    Vector2 moveInput;
    bool pp;
    public override void Player_Action2_performed(InputAction.CallbackContext context) 
    {
        if (!pp)
        {
            pp = true;
            onRun = !onRun;
            InvokePlayerStateChangeEvent_Editor();
        }
    }
    public override void Player_Action2_canceled(InputAction.CallbackContext context)
    {
        pp = false;
    }

    public override void Player_Move_performed(InputAction.CallbackContext context) 
    {
        moveInput = context.ReadValue<Vector2>();
        moveInput.y = 0;
        onMoveInput = true;
    }

    public override void Player_Move_canceled(InputAction.CallbackContext context) 
    {
        onMoveInput = false;
    }
    #endregion

    [ContextMenu("Editor: Invoke Player State Change Evt")]
    public void InvokePlayerStateChangeEvent_Editor()
    {
        OnPlayStateChanged?.Invoke(this, new PlayerStateChangeEventArgs
        {
            onRunning = onRun,
            speed = moveSpeed
        });

    }
}
