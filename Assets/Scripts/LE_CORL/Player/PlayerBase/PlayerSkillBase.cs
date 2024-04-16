using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.LE_CORL.Player
{
    public abstract class PlayerSkillBase : ScriptableObject
    {
        public enum SkillState { Ready, OnActiating, OnCooldonw, };


        [SerializeField] float skillCooldown;

        protected PlayerControlIsomatric player;
        public SkillState m_SkillState { get; protected set; }


        public virtual void Init(PlayerControlIsomatric playerMain)
        {
            this.player = playerMain;
            SetStateReady();
        }


        public void ActivateSkill()
        {
            if (m_SkillState != SkillState.Ready)
                return;
            player.StartCoroutine(SkillLogic());
        }

        protected abstract IEnumerator SkillLogic();

        protected void SetStateCooldonw()
        {
            m_SkillState = SkillState.OnCooldonw;
            player.Invoke("SetStateReady", skillCooldown);
        }
        void SetStateReady() => m_SkillState = SkillState.Ready;
    }
}