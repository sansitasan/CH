using System.Collections;
using UnityEngine;

namespace Assets.Scripts.LE_CORL.Core
{
    public class PauseCtrl : MonoBehaviour, ICore
    {
        public bool IsIntitalized => playerInputAction != null;
        public static bool OnPause { get; private set; }
        bool onPause;

        PlayerInputAction playerInputAction;

        public void Init()
        {
            onPause = false;
        }

        public static void SetPause()
        {
            OnPause = !OnPause;
            Time.timeScale = OnPause ? 1.0f : 0.0f;

            GameMainContoller.Instance.GamePause();
        }

        public void Disable()
        {

        }
    }
}