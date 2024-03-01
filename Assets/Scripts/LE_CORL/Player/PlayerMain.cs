using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMain : MonoBehaviour
{
    [SerializeField] float skillCooldonw;
    [SerializeField] float moveSpeed;

    bool canMove;
    Vector2 moveInput;
    PlayerInputAction inputAction;

    private void Awake()
    {
    }


    private void OnEnable()
    {
        inputAction = new PlayerInputAction();
        var isomatric = inputAction.Player_Isometric;
        isomatric.Enable();

        isomatric.Move.performed += Move_performed;
        isomatric.Move.canceled += Move_canceled;

        isomatric.Interaction.performed += Interaction_performed;
        isomatric.Interaction.canceled += Interaction_canceled;

        isomatric.Alt_Interaction.performed += Alt_Interaction_performed;
        isomatric.Alt_Interaction.canceled += Alt_Interaction_canceled;
    }

    private void Start()
    {

    }


    private void FixedUpdate()
    {
        if(canMove)
            transform.Translate(Time.deltaTime * moveSpeed * moveInput);
    }

    private void OnDisable()
    {
        inputAction.Player_Isometric.Disable();
    }

    private void Move_performed(InputAction.CallbackContext obj)
    {
        canMove = true;
        moveInput = obj.ReadValue<Vector2>().normalized;
    }

    private void Move_canceled(InputAction.CallbackContext obj)
    {
        canMove = false;
        moveInput = Vector2.zero;
    }

    private void Interaction_performed(InputAction.CallbackContext obj)
    {
        print("input - isomatric - interation(space bar) - performed");
    }
    private void Interaction_canceled(InputAction.CallbackContext obj)
    {
        print("input - isomatric - interaction(space bar) - canceled");
    }
    private void Alt_Interaction_performed(InputAction.CallbackContext obj)
    {
        print("input - isomatric - altInteration(m) - performed");
    }

    private void Alt_Interaction_canceled(InputAction.CallbackContext obj)
    {
        print("input - isomatric - altInteration(m) - canceled");
    }
}
