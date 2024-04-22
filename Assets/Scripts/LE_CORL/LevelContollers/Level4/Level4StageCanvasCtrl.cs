using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class Level4StageCanvasCtrl : MonoBehaviour
{
    [Header("Timer")]
    [SerializeField] float highlightTime = 5;
    [SerializeField] Color baseColor;
    [SerializeField] Color highlightColor;
    [SerializeField] TextMeshProUGUI timerText;

    [Header("Player Health")]
    [SerializeField] GameObject playerHealthBundle;
    [SerializeField] TextMeshProUGUI playerHPtext;

    [Header("Arrow")]
    [SerializeField] GameObject arrow;
    [SerializeField] float arrowDuration;

    CancellationTokenSource token;

    private void Awake()
    {
        SetCanvasClear();
        arrow.SetActive(false);
    }

    private void OnEnable()
    {
        Level4RoomController.OnAnyRoomStateChanged += Level4RoomController_OnAnyRoomStateChanged;
        Level4MainContoller.OnPlayerHPUpdated += Level4MainContoller_OnPlayerHit;
    }

    private void Level4MainContoller_OnPlayerHit(object sender, Level4MainContoller.PlayerHPUpdaterEventArgs e)
    {
        playerHPtext.text = $"x{e.current}";
    }

    private void OnDisable()
    {
        Level4RoomController.OnAnyRoomStateChanged -= Level4RoomController_OnAnyRoomStateChanged;
        Level4MainContoller.OnPlayerHPUpdated -= Level4MainContoller_OnPlayerHit;
    }

    private void Level4RoomController_OnAnyRoomStateChanged(object sender, Level4RoomController.RoomStateChangedEventArgs e)
    {
        if (!e.isStarted)
        {
            SetCanvasClear();

            if(e.isCleared)
                SetArrow().Forget();

            return;
        }

        int duration = e.ruleset.roomDuration;

        if (token != null)
            token.Dispose();

        token = new CancellationTokenSource();

        playerHealthBundle.SetActive(true);
        SetTimer(duration, token).Forget();
    }

    void SetCanvasClear()
    {
        timerText.text = string.Empty;
        timerText.color = baseColor;
        playerHealthBundle.SetActive(false);
        if(token != null)
            token.Cancel();
    }

    async UniTask SetTimer(int duration, CancellationTokenSource cancelToken)
    {
        while (duration >= 0 && !cancelToken.IsCancellationRequested)
        {
            timerText.text = duration.ToString();
            if (duration <= highlightTime)
                timerText.color = highlightColor;

            await UniTask.Delay(TimeSpan.FromSeconds(1f));
            duration -= 1;
        }
    }

    async UniTask SetArrow()
    {
        var token = this.GetCancellationTokenOnDestroy();
        arrow.SetActive(true);

        await UniTask.WhenAny(
            UniTask.Delay(TimeSpan.FromSeconds(arrowDuration)),
            UniTask.WaitUntil(() => token.IsCancellationRequested)
            );

        arrow.SetActive(false);
    }
}
