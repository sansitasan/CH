using Cysharp.Threading.Tasks;
using System;
using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Level4RoomController : MonoBehaviour
{
    [SerializeField] Level4RoomRuleset myRoomRuleset;
    [SerializeField] PlayerEventTrigger myEventTrigger;

    bool isPlaying;

    Queue<int> fallingObjectPrioQueue;
    CancellationTokenSource patternCancellationToken;

    public static event EventHandler<RoomStateChangedEventArgs> OnAnyRoomStateChanged;
    public class RoomStateChangedEventArgs : EventArgs
    {
        public bool isStarted;
        public Level4RoomRuleset ruleset;
    }

    private void Start()
    {
        PlayerEventTrigger.OnPlayerEntered += PlayerEventTrigger_OnPlayerEntered;
        if(myRoomRuleset.isEscapeableRoom)
            PlayerEventTrigger.OnPlayerExited += PlayerEventTrigger_OnPlayerExited;

        myEventTrigger.SetTriggerActivation(true);
    }
    private void PlayerEventTrigger_OnPlayerEntered(object sender, EventArgs e)
    {
        if (isPlaying)
            return;

        var tmp = (PlayerEventTrigger)sender;
        if (tmp != myEventTrigger)
            return;


        OnAnyRoomStateChanged?.Invoke(this, new RoomStateChangedEventArgs
        {
            isStarted = true,
            ruleset = myRoomRuleset
        });

        patternCancellationToken = new CancellationTokenSource();
        PlayRoomPattern(patternCancellationToken).Forget();

        isPlaying = true;
    }

    private void PlayerEventTrigger_OnPlayerExited(object sender, EventArgs e)
    {
        if (!isPlaying)
            return;

        var tmp = (PlayerEventTrigger)sender;
        if (tmp != myEventTrigger)
            return;

        patternCancellationToken.Cancel();
    }

    int GetNextFallingObjectID()
    {
        var next = fallingObjectPrioQueue.Dequeue();
        fallingObjectPrioQueue.Enqueue(next);
        return next;
    }


    async UniTaskVoid PlayRoomPattern(CancellationTokenSource cancellation)
    {
        float startTick = Time.time;
        float finalTick = startTick + (float)myRoomRuleset.roomDuration - myRoomRuleset.generationTick;
        int countMax = myRoomRuleset.generationMax;
        AnimationCurve ratio = myRoomRuleset.generationRatio;

        float progress = 0f;
        Vector2 pointA = myRoomRuleset.pointA;
        Vector2 pointB = myRoomRuleset.pointB;
        float minDistance = myRoomRuleset.minDistancing;

        fallingObjectPrioQueue = myRoomRuleset.GetRandomizedFallingObjectQueue();
        
        await UniTask.WaitUntil (() => fallingObjectPrioQueue != null);

        // 패턴
        while (!cancellation.IsCancellationRequested && progress <= 1.0f)
        {
            int generationCount = Mathf.RoundToInt(Mathf.Clamp(ratio.Evaluate(progress), 1, countMax));

            // 위지 지정
            List<Vector2> positions = new List<Vector2>();
            while (positions.Count < generationCount)
            {
                Vector2 randomPos;
                randomPos.x = Mathf.Lerp(pointA.x, pointB.x, UnityEngine.Random.value);
                randomPos.y = Mathf.Lerp(pointA.y, pointB.y, UnityEngine.Random.value);

                foreach (Vector2 pos in positions)
                {
                    if (Vector2.Distance(pos, randomPos) <= minDistance)
                        continue;
                }
                positions.Add(randomPos);
            }

            // 생성
            foreach (var pos in positions)
            {
                var targetIDX = GetNextFallingObjectID();
                var go = MemoryPoolManager.GetGameObject(Level4MainContoller.FALLING_OBJECT_ID + targetIDX);
                go.transform.position = pos;
                go.SetActive(true);
            }

            // 진행도 계산
            progress = 1 - (finalTick - Time.time) / (finalTick - startTick);

            // 프레임 진행
            await UniTask.Delay(TimeSpan.FromSeconds(myRoomRuleset.generationTick));
        }

        // 후처리
        DiposeSelf();
    }

    void DiposeSelf()
    {
        OnAnyRoomStateChanged?.Invoke(this, new RoomStateChangedEventArgs
        {
            isStarted = false,
            ruleset = null,
        });
        myEventTrigger.SetTriggerActivation(false);
        fallingObjectPrioQueue = null;
        patternCancellationToken.Dispose();
        patternCancellationToken = null;
        
        PlayerEventTrigger.OnPlayerEntered -= PlayerEventTrigger_OnPlayerEntered;
        if (myRoomRuleset.isEscapeableRoom)
            PlayerEventTrigger.OnPlayerExited -= PlayerEventTrigger_OnPlayerExited;

        Destroy(gameObject);
    }

    public void SetRoomCtrl(Level4RoomRuleset ruleset, PlayerEventTrigger trigger)
    {
        myRoomRuleset = ruleset;
        myEventTrigger = trigger;
    }
}
