using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class Level4PlayerAnimation : MonoBehaviour
{
    [SerializeField] Sprite[] left;
    [SerializeField] Sprite[] right;
    [SerializeField] Sprite[] up;
    [SerializeField] Sprite[] down;
    [SerializeField] Texture2D texture2d;

    SpriteRenderer m_SpriteRenderer;
    Level4PlayerController controller;

    private void Awake()
    {
        m_SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        m_SpriteRenderer.sprite = left[0];
        controller = GetComponent<Level4PlayerController>();
        PlayerControllerBase.OnPlayerSkillStateChanged += PlayerControllerBase_OnPlayerSkillStateChanged;
    }
    private void OnDisable()
    {
        PlayerControllerBase.OnPlayerSkillStateChanged -= PlayerControllerBase_OnPlayerSkillStateChanged;
    }

    private void Update()
    {
        if (controller == null) return;

        /*
        if(controller.MoveInput.y != 0)
        {
            m_SpriteRenderer.sprite = controller.MoveInput.y > 0 ? up[0] : down[0];
        }
        */
        if (controller.MoveInput.x != 0)
        {
            m_SpriteRenderer.sprite = controller.MoveInput.x > 0 ? right[0] : left[0];
        }
    }

    private void PlayerControllerBase_OnPlayerSkillStateChanged(object sender, Assets.Scripts.LE_CORL.Player.PlayerSkillBase.SkillState e)
    {

    }


}
