using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageData : ScriptableObject
{
    [Header("속도"), Range(1f, 20f)]
    public float Speed;
}
