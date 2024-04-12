using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level4PlayerSkill : MonoBehaviour
{


    SpriteRenderer m_spriteRenderer;
    PlayerControllerBase ctrl;
    float predelay;

    public void InitSkillInfo(PlayerControllerBase player, float predelay)
    {
        ctrl = player;
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        this.predelay = predelay;
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        
    }
}