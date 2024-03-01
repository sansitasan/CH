using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Build.Pipeline.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class Level4RoomController : MonoBehaviour
{
    Level4RoomRuleset currentRoomRuleset;

    const string FALLING_OBSTACLE_ID = "falling_obstacle";
    [SerializeField] GameObject fallingObstaclePrefab;
    [SerializeField] Level4MainContoller mainController;

    IEnumerator roomPattern = null;

    private void OnEnable()
    {
        
        // 방 진입 이벤트 등록
        mainController.OnRoomStateChanged += MainController_OnRoomStateChanged;
    }

    private void Start()
    {
        // 메모리풀 등록
        MemoryPoolManager.Instance.RegisterMemorypoolObj(FALLING_OBSTACLE_ID, fallingObstaclePrefab);
    }

    private void MainController_OnRoomStateChanged(object sender, Level4MainContoller.RoomStateChangedEventArgs e)
    {
        if (e.isStartRoom)
        {
            currentRoomRuleset = e.roomRulesetData;
            roomPattern = RoomPattern();
            StartCoroutine(roomPattern);
        }
        else
        {
            currentRoomRuleset = null;
            StopCoroutine(roomPattern);
            roomPattern = null;
        }
    }

    IEnumerator RoomPattern()
    {
        var generationDelay = new WaitForSeconds(currentRoomRuleset.fallingObtaclesGenerationTick);

        // float finalGenerationTime = 

        // Queue<(List<Vector2>, float)> actingPos = new Queue<(List<Vector2>, float)>();
        float startTick = Time.time;
        float finalTick = startTick + (float)currentRoomRuleset.roomDuration - currentRoomRuleset.fallingObtaclesGenerationTick;
        int countMax = currentRoomRuleset.fallingObstaclesCountMax;
        AnimationCurve ratio = currentRoomRuleset.fallingObstacleRatio;

        float progress = 0f;
        Vector2 pointA = currentRoomRuleset.pointA;
        Vector2 pointB = currentRoomRuleset.pointB;
        float minDistance = currentRoomRuleset.fallingObstaclesMinDistancing;

        while (progress <= 1)
        {
            // 개수 지정
            int count = Mathf.RoundToInt(Mathf.Clamp(ratio.Evaluate(progress), 1, countMax));

            // 위지 지정
            List<Vector2> positions = new List<Vector2>();
            while(positions.Count < count)
            {
                Vector2 randomPos;
                randomPos.x = Mathf.Lerp(pointA.x, pointB.x, UnityEngine.Random.value);
                randomPos.y = Mathf.Lerp(pointA.y, pointB.y, UnityEngine.Random.value);

                foreach(Vector2 pos in positions)
                {
                    if (Vector2.Distance(pos, randomPos) <= minDistance)
                        continue;
                }
                positions.Add(randomPos);
            }

            // 생성
            foreach (var pos in positions)
            {
                var go = MemoryPoolManager.Instance.GetGameObject(FALLING_OBSTACLE_ID);
                go.transform.position = pos;
                go.SetActive(true);
            }

            // 진행도 설정
            progress = ( finalTick - Time.time ) / (finalTick - startTick );

            // 큐 정리

            // 프레임 진행
            yield return generationDelay;
        }

        yield break;
    }
}
