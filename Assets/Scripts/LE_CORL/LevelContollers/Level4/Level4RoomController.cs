using Cinemachine;
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
    public Transform myRespawnPosition;
    public CinemachineVirtualCamera myCamera;

    bool isPlaying;

    Queue<int> fallingObjectPrioQueue;
    CancellationTokenSource patternCancellationToken;

    public static event EventHandler<RoomStateChangedEventArgs> OnAnyRoomStateChanged;
    public class RoomStateChangedEventArgs : EventArgs
    {
        public bool isStarted;
        public bool isCleared;
        public Level4RoomRuleset ruleset;
    }

    private void Start()
    {
        myCamera.gameObject.SetActive(false);
        PlayerEventTrigger.OnPlayerEntered += PlayerEventTrigger_OnPlayerEntered;
        Level4MainContoller.OnPlayerHPUpdated += Level4MainContoller_OnPlayerHPUpdated;
        myEventTrigger.SetTriggerActivation(true);
    }

    private void Level4MainContoller_OnPlayerHPUpdated(object sender, Level4MainContoller.PlayerHPUpdaterEventArgs e)
    {
        if (e.current > 0) return;
        if(!isPlaying) return;

        patternCancellationToken.Cancel();
    }

    // room 트라이에 실패
    private void PlayerEventTrigger_OnPlayerExited(object sender, EventArgs e)
    {
        if (!isPlaying) return;
        var tmp = (PlayerEventTrigger)sender;
        if (tmp != myEventTrigger)
            return;

        patternCancellationToken.Cancel();
    }

    // 방 진입
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

        if(patternCancellationToken != null)
            patternCancellationToken.Dispose();

        patternCancellationToken = new CancellationTokenSource();
        PlayRoomPattern(patternCancellationToken).Forget();
    }


    int GetNextFallingObjectID()
    {
        var next = fallingObjectPrioQueue.Dequeue();
        fallingObjectPrioQueue.Enqueue(next);
        return next;
    }


    async UniTaskVoid PlayRoomPattern(CancellationTokenSource cancellation)
    {
        isPlaying = true;

        float startTick = Time.time;
        float finalTick = startTick + (float)myRoomRuleset.roomDuration - myRoomRuleset.generationTick;
        var start = Time.realtimeSinceStartupAsDouble;

        int countMax = myRoomRuleset.generationMax;
        AnimationCurve ratio = myRoomRuleset.generationRatio;

        float progress = 0f;
        Vector2 pointA = myRoomRuleset.pointA;
        Vector2 pointB = myRoomRuleset.pointB;
        float minDistance = myRoomRuleset.minDistancing;

        fallingObjectPrioQueue = myRoomRuleset.GetRandomizedFallingObjectQueue();
        await UniTask.WhenAll(
            UniTask.WaitUntil(() => fallingObjectPrioQueue != null),
            UniTask.Delay(TimeSpan.FromSeconds(Level4MainContoller.Instance.CinemachineBlendDuration))
            );


        // 패턴
        while (!cancellation.IsCancellationRequested && progress <= 1.0f)
        {
            int generationCount = Mathf.RoundToInt(Mathf.Clamp(ratio.Evaluate(progress), 1, countMax));
            float fallingObjectRadius = 1f;
            int wallMask = 1 << LayerMask.NameToLayer("Wall");

            // 위지 지정
            List<Vector2> positions = new List<Vector2>();
            Vector2 randomPos;

            while (positions.Count < generationCount)
            {
                randomPos.x = Mathf.Lerp(pointA.x, pointB.x, UnityEngine.Random.value);
                randomPos.y = Mathf.Lerp(pointA.y, pointB.y, UnityEngine.Random.value);

                foreach (Vector2 pos in positions)
                    if (Vector2.Distance(pos, randomPos) <= minDistance)
                        continue;

                if (Physics2D.OverlapCircle(randomPos, fallingObjectRadius, wallMask))
                    continue;

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
        var end = Time.realtimeSinceStartupAsDouble;
        print(end - start);

        bool isCleared = !cancellation.IsCancellationRequested;
        if(isCleared)
            await UniTask.Delay(TimeSpan.FromSeconds(Level4MainContoller.Instance.RoomPostDelay));

        isPlaying = false;

        OnAnyRoomStateChanged?.Invoke(this, new RoomStateChangedEventArgs
        {
            isStarted = false,
            isCleared = isCleared,
            ruleset = null,
        });
        if (isCleared)
            gameObject.SetActive(false);

    }

    private void OnDisable()
    {
        myEventTrigger.SetTriggerActivation(false);
        fallingObjectPrioQueue = null;
        patternCancellationToken?.Dispose();
        patternCancellationToken = null;
        PlayerEventTrigger.OnPlayerEntered -= PlayerEventTrigger_OnPlayerEntered;
    }


    // 에디터 용
    public void SetRoomCtrl(Level4RoomRuleset ruleset, PlayerEventTrigger trigger, CinemachineVirtualCamera cam, Transform respawn)
    {
        myRoomRuleset = ruleset;
        myEventTrigger = trigger;
        myCamera = cam;
        myRespawnPosition = respawn;
    }
}
