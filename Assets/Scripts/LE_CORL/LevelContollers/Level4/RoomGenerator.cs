using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RoomGenerator : MonoBehaviour
{
    public const string ROOM_DATA_PATH = "Assets/Scripts/LE_CORL/Datas/Level4RoomDatas/";

    public Transform roomTriggerBundle;
    public Transform pointA, pointB;

    public int roomDuration = 30;
    public int fallingObstaclesCountMax = 10;
    public float fallingObstaclesMinDistancing = 1;
    public float fallingObstclesMinTimeGap = .2f;
    public AnimationCurve fallingObstacleRatio;

    private void Awake()
    {
#if UNITY_EDITOR

#else
        Destroy(gameObject);
#endif
    }
}
