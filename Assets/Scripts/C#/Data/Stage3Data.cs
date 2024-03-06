using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage3Data : StageData
{
    [Header("오브젝트 움직임 속도"), Range(0.1f, 20f)]
    public float ObjectMoveSpeed;
    [Header("첫번째 밀 수 있는 횟수")]
    public int MoveCount_1;
    [Header("두번째 밀 수 있는 횟수")]
    public int MoveCount_2;
}
