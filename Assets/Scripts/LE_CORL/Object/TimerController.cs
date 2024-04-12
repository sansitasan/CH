using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class TimerController : MonoBehaviour
{
    [SerializeField] float highlightTime = 5;
    [SerializeField] Color baseColor;
    [SerializeField] Color highlightColor;

    TextMeshProUGUI mText;
    CancellationTokenSource token;

    private void Awake()
    {
        mText = GetComponent<TextMeshProUGUI>();
        SetEmpty();
    }

    private void Start()
    {
        Level4RoomController.OnAnyRoomStateChanged += Level4RoomController_OnAnyRoomStateChanged;
    }
    private void OnDisable()
    {
        Level4RoomController.OnAnyRoomStateChanged -= Level4RoomController_OnAnyRoomStateChanged;
    }

    private void Level4RoomController_OnAnyRoomStateChanged(object sender, Level4RoomController.RoomStateChangedEventArgs e)
    {
        if(!e.isStarted)
        {
            SetEmpty();
            token.Cancel();
            return;
        }

        int duration = e.ruleset.roomDuration;
        token = new CancellationTokenSource();
        SetTimer(duration, token).Forget();
    }


    void SetEmpty()
    {
        mText.text = string.Empty;
        mText.color = baseColor;
    }

    async UniTask SetTimer(int duration, CancellationTokenSource cancelToken)
    {
        while (duration >= 0 && !cancelToken.IsCancellationRequested)
        {
            mText.text = duration.ToString();
            if (duration <= highlightTime)
                mText.color = highlightColor;

            await UniTask.Delay(TimeSpan.FromSeconds(1f));
            duration -= 1;
        }
    }
}
