using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1Data : StageData
{
    [Header("차지 단계에 따른 점프력, 차지 시간"), Range(1, 100)]
    public int[] ChargePower;
    [Range(0.5f, 2f)]
    public float ChargeTime;
    [Header("중력"), Range(0.5f, 100f)]
    public float GravityScale;

    [Header("스킬 쿨타임"), Range(0, 100)]
    public float SkillCoolTime;

    [Header("벽, 바닥 물리 머티리얼")]
    public PhysicsMaterial2D GroundMaterial;
    public PhysicsMaterial2D WallMaterial;
}
