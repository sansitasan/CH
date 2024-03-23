using Assets.Scripts.LE_CORL.Player;
using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerControlIsomatric : MonoBehaviour
{
    const string ANIMATOR_KEY_IS_MOVE = "Move";
    const string ANIMATOR_KEY_Y = "Y";

    [field: SerializeField] public CinemachineVirtualCamera vCam { get; private set; }
    [field: SerializeField] public SpriteRenderer playerRenderer { get; private set; }

    [Space(15)]
    [SerializeField] float moveSpeed;
    [SerializeField] float camFov;
    [field: SerializeField] public PlayerSkillBase skill { get; private set; }
    
    Animator playerAnimator;

    bool onMove;
    Vector2 moveInput;
    PlayerInputAction.Player_IsometricActions playerInput;


    private void Awake()
    {
    }


    private void OnEnable()
    {
        // Base Setup
        vCam.m_Lens.OrthographicSize = camFov;
        playerAnimator = playerRenderer.GetComponent<Animator>();
        skill.Init(this);

        // Input Setup
        PlayerInputAction input = new PlayerInputAction();

        playerInput = input.Player_Isometric;
        playerInput.Move.performed += Move_performed;
        playerInput.Move.canceled += Move_canceled;
        playerInput.Action1.performed += Interaction_performed;
        playerInput.Action1.canceled += Interaction_canceled;
        playerInput.Action2.performed += Alt_Interaction_performed;
        playerInput.Action2.canceled += Alt_Interaction_canceled;
        playerInput.Enable();
    }

    private void Update()
    {
        playerAnimator.SetBool(ANIMATOR_KEY_IS_MOVE, onMove);
        int parseY = moveInput.y > 0 ? 1 : moveInput.y < 0 ? -1 : 0;
        playerAnimator.SetInteger(ANIMATOR_KEY_Y, parseY);
        playerRenderer.flipX = moveInput.x > 0 ? false : moveInput.x < 0 ? true : playerRenderer.flipX;
    }

    private void FixedUpdate()
    {
        if(onMove)
            transform.Translate(Time.deltaTime * moveSpeed * moveInput);
    }

    private void OnDisable()
    {
        playerInput.Move.performed -= Move_performed;
        playerInput.Move.canceled -= Move_canceled;
        playerInput.Action1.performed -= Interaction_performed;
        playerInput.Action1.canceled -= Interaction_canceled;
        playerInput.Action2.performed -= Alt_Interaction_performed;
        playerInput.Action2.canceled -= Alt_Interaction_canceled;
        playerInput.Disable();
    }


    #region PlayerInput
    // Target Key: WASD
    private void Move_performed(InputAction.CallbackContext obj)
    {
        onMove = true;
        moveInput = obj.ReadValue<Vector2>().normalized;
    }

    private void Move_canceled(InputAction.CallbackContext obj)
    {
        onMove = false;
    }

    // Target Key: "SpaceBar"
    private void Interaction_performed(InputAction.CallbackContext obj)
    {
        print("input - isomatric - interation(space bar) - performed");
    }
    private void Interaction_canceled(InputAction.CallbackContext obj)
    {
        print("input - isomatric - interaction(space bar) - canceled");
    }

    // Target Key: "m"
    private void Alt_Interaction_performed(InputAction.CallbackContext obj)
    {
        skill.ActivateSkill();

        print("input - isomatric - altInteration(m) - performed");
    }

    private void Alt_Interaction_canceled(InputAction.CallbackContext obj)
    {
        print("input - isomatric - altInteration(m) - canceled");
    }
    #endregion

}
