using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUIController : MonoBehaviour
{
    [SerializeField] Selectable first;
    [SerializeField] Slider[] sliders;
    [SerializeField] Button[] buttons;


    bool isLobby;
    Selectable last = null;

    private void OnEnable()
    {
        isLobby = SceneManager.GetActiveScene().buildIndex == GameMainContoller.LOBBY_SCENE_IDX ? true : false;

        print("todo: 슬라이더 벨류 받아오기");
        
        sliders[0].value = .5f;
        sliders[0].onValueChanged.AddListener((v) => OnSliderValueChanged(v));

        sliders[1].value = .5f;
        sliders[1].onValueChanged.AddListener((v) => OnSliderValueChanged(v));

        sliders[2].value = .5f;
        sliders[2].onValueChanged.AddListener((v) => OnSliderValueChanged(v));

        if(isLobby)
        {
            buttons[0].onClick.AddListener(() => GameMainContoller.Instance.GamePause()); // 퍼즈 풀기
            buttons[0].GetComponentInChildren<TextMeshProUGUI>().text = "게임으로 돌아가기";

            buttons[1].onClick.AddListener(() => GameMainContoller.Instance.LoadScene(0)); // 타이틀로 돌아가기
            buttons[1].GetComponentInChildren<TextMeshProUGUI>().text = "타이틀로 돌아가기";
        }
        else
        {
            buttons[0].onClick.AddListener(() => GameMainContoller.Instance.ReloadScene()); // 퍼즈 풀기
            buttons[0].GetComponentInChildren<TextMeshProUGUI>().text = "다시하기";

            buttons[1].onClick.AddListener(() => GameMainContoller.Instance.LoadLobby()); // 로비
            buttons[1].GetComponentInChildren<TextMeshProUGUI>().text = "로비로 돌아가기";
        }
        first.Select();
    }

    private void OnDisable()
    {
        sliders[0].onValueChanged.RemoveAllListeners();
        sliders[1].onValueChanged.RemoveAllListeners();
        sliders[2].onValueChanged.RemoveAllListeners();
        buttons[0].onClick.RemoveAllListeners();
        buttons[1].onClick.RemoveAllListeners();
    }

    void OnSliderValueChanged(float v) 
    {
        print("테스트 코드입니다.\n메뉴-슬라이더-벨류체인지 / value: " + v);
        print("todo: 옵션에 연결");
    }

}