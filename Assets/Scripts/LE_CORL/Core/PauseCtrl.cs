using System.Collections;
using UnityEngine;

namespace Assets.Scripts.LE_CORL.Core
{
    public class PauseCtrl : MonoBehaviour, ICore
    {
        public bool IsIntitalized => playerInputAction != null;

        PlayerInputAction playerInputAction;
        bool onPause;

        public void Init()
        {
            onPause = false;
            Init_Input();
        }

        void Init_Input()
        {
            playerInputAction = new PlayerInputAction();
            playerInputAction.Enable();
            playerInputAction.UI.Enable();
            playerInputAction.UI.Escape.performed += Escape_performed;
        }

        private void OnDisable()
        {

        }
        private void Escape_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            onPause = !onPause;
            Time.timeScale = onPause ? 0 : 1f;
            GameMainContoller.Instance.LoadMenuScene();
        }

        public void Disable()
        {
            playerInputAction.UI.Escape.performed -= Escape_performed;
            playerInputAction.UI.Disable();
            playerInputAction.Disable();
        }
    }
}