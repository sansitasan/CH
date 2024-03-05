using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.LE_CORL.Player
{
    public class PlayerSkillBase : ScriptableObject
    {
        public enum SkillState { Ready, OnActiating, OnCooldonw, };


        [SerializeField] float skillCooldown;

        protected PlayerControlIsomatric playerMain;
        public SkillState m_SkillState { get; protected set; }


        public virtual void Init(PlayerControlIsomatric playerMain)
        {
            this.playerMain = playerMain;
            SetStateReady();
        }


        public void ActivateSkill()
        {
            if (m_SkillState != SkillState.Ready)
                return;
            playerMain.StartCoroutine(SkillLogic());
        }

        protected virtual IEnumerator SkillLogic() 
        {
            m_SkillState = SkillState.OnActiating;

            // Skill action logic
            yield return null;

            SetStateCooldonw();
        }

        protected void SetStateCooldonw()
        {
            m_SkillState = SkillState.OnCooldonw;
            playerMain.Invoke("SetStateReady", skillCooldown);
        }
        void SetStateReady() => m_SkillState = SkillState.Ready;
    }
}