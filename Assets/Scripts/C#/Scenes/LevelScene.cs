using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelScene : MonoBehaviour
{
    [SerializeField]
    private Button _startButton;
    [SerializeField] 
    private Button[] _levelButtons;


    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        _startButton.onClick.AddListener(() => ChangeScene(0));
        for (int i = 0; i < _levelButtons.Length; ++i)
        {
            int temp = i;
            _levelButtons[i].onClick.AddListener(() => ChangeScene(temp + 2));
        }
        GameMainContoller.Instance.ChangeScene += Clear;
    }

    private void Start()
    {
        _levelButtons[0].Select();
    }



    private void ChangeScene(int Scene)
    {
        GameMainContoller.Instance.LoadScene(Scene);
    }

    private void Clear()
    {
        GameMainContoller.Instance.ChangeScene -= Clear;
        _startButton.onClick.RemoveAllListeners();
        
        for (int i = 0; i < _levelButtons.Length; ++i)
            _levelButtons[i].onClick.RemoveAllListeners();
    }
}
