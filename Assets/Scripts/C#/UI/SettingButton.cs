using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingButton : MonoBehaviour
{
    private Button _settingButton;

    private void Start()
    {
        _settingButton = GetComponent<Button>();
        _settingButton.onClick.AddListener(() => GameMainContoller.Instance.GamePause());
    }

    private void OnDestroy()
    {
        _settingButton.onClick.RemoveAllListeners();
    }
}
