using System.Collections;
using UnityEngine;

namespace Assets.Scripts.LE_CORL.Player
{
    public class Level4PlayerSkill : PlayerSkillBase
    {
        [SerializeField] Color targetColor;
        [SerializeField] float duration;

        protected override IEnumerator SkillLogic()
        {
            m_SkillState = SkillState.OnActiating;

            SpriteRenderer playerRenderer = playerMain.playerRenderer;

            // Skill action logic
            playerRenderer.color = targetColor;

            yield return new WaitForSeconds(duration);

            playerRenderer.color = Color.white; 

            SetStateCooldonw();
        }
    }
}