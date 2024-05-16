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
        SoundVolume volume = GameMainContoller.GetCore<SoundManager>().Volume;
        
        sliders[0].value = volume.TotalVolume;
        sliders[0].onValueChanged.AddListener((v) => OnSliderValueChanged(v, ESound.Total));

        sliders[1].value = volume.BGMVolume;
        sliders[1].onValueChanged.AddListener((v) => OnSliderValueChanged(v, ESound.Bgm));

        sliders[2].value = volume.EffectVolume;
        sliders[2].onValueChanged.AddListener((v) => OnSliderValueChanged(v, ESound.Effect));

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
        GameMainContoller.GetCore<SoundManager>().SaveSound(new SoundVolume(sliders[0].value, sliders[1].value, sliders[2].value));
    }

    void OnSliderValueChanged(float v, ESound type) 
    {
        print("테스트 코드입니다.\n메뉴-슬라이더-벨류체인지 / value: " + v);
        print("todo: 옵션에 연결");
        GameMainContoller.GetCore<SoundManager>().SetVolumeTemporary(v, type);
    }

}