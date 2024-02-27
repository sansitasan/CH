using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMain : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }



    Vector2 moveInput;

    private void Awake()
    {
    }

    private void OnEnable()
    {
        PlayerInputAction inputActions = new PlayerInputAction();
        var isomatric = inputActions.Player_Isometric;
        isomatric.Enable();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
