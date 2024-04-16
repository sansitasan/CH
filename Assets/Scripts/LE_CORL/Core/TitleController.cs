using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleController : MonoBehaviour
{
    [SerializeField] Button startButton;

    private void Awake()
    {
        startButton.gameObject.SetActive(false);
    }

    private void Start()
    {
        ActivateButton().Forget();
    }

    async UniTaskVoid ActivateButton()
    {
        await UniTask.WaitUntil(() => GameMainContoller.IsIntitalized);
        startButton.gameObject.SetActive(true);
        startButton.Select();
    }

    public void StartBtnClicked()
    {
        GameMainContoller.Instance.LoadScene(1);
        print("btn click - game start");
    }
}
