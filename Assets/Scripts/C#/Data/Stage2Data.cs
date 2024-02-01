using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Stage2Data : StageData
{
    [Header("스킬 설정")]
    [Header("페이드 시간"), Range(0.1f, 10f)]
    public  float FadeInTime;
    [Range(0.1f, 10f)]
    public float FadeOutTime;
    [Header("최대 크기에서 머무르는 시간"), Range(0.1f, 10f)]
    public float LightOnTime;
    [Header("쿨타임"), Range(0.1f, 30f)]
    public float SkillCoolTime;

    [Header("라이트 설정")]
    [Header("밝기 강도"), Range(0, 2f)]
    public float Intensity;
    [Header("작은 원 크기"), Range(0, 10f)]
    public float InnerCircleSize;
    [Header("큰 원 크기"), Range(1f, 10f)]
    public float OuterCircleSize;
    [Header("시야 조정"), Range(0, 1f)]
    public float FallOffStrength;
    [Header("스킬 사용 시 원 최대 크기"), Range(1f, 20f)]
    public float MaxCircleSize;
}
