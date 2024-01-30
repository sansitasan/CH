using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Stage2Data : StageData
{
    [Header("페이드 시간"), Range(0.1f, 10f)]
    public float[] FadeTime;
    [Header("쿨타임"), Range(0.1f, 30f)]
    public float Cooltime;
    [Header("횟수"), Range(1, 100)]
    public int SkillCount;
}
