using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerController : MonoBehaviour
{
    [SerializeField] float highlightTime = 5;
    [SerializeField] Color baseColor;
    [SerializeField] Color highlightColor;

    TextMeshProUGUI mText;
    IEnumerator timerCoroutin = null;

    private void Awake()
    {
        mText = GetComponent<TextMeshProUGUI>();
        SetEmpty();
    }

    private void Start()
    {
        Level4MainContoller.OnRoomStateChanged += Level4MainContoller_OnRoomStateChanged;
    }
    private void OnDisable()
    {
        Level4MainContoller.OnRoomStateChanged -= Level4MainContoller_OnRoomStateChanged;
    }

    private void Level4MainContoller_OnRoomStateChanged(object sender, Level4MainContoller.RoomStateChangedEventArgs e)
    {
        if (!e.isStartRoom)
            return;

        int duration = e.roomRulesetData.roomDuration;
        timerCoroutin = SetTimer(duration);
        StartCoroutine(timerCoroutin);
    }

    void SetEmpty()
    {
        mText.text = string.Empty;
        mText.color = baseColor;
    }

    IEnumerator SetTimer(int duration)
    {
        var _1s = new WaitForSeconds(1);
        while(duration >= 0)
        {
            mText.text = duration.ToString();
            if (duration <= highlightTime)
                mText.color = highlightColor;

            yield return _1s;
            duration -= 1;
        }
    }
}
