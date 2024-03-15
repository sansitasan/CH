using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Room (Level4)", menuName = "Datas/Level4 Room Ruleset", order = 0)]
public class Level4RoomRuleset : ScriptableObject
{
    public Vector2 pointA, pointB;
    public int roomDuration = 30;
    public int fallingObstaclesCountMax = 10;
    public float fallingObstaclesMinDistancing = 1;
    public float fallingObtaclesGenerationTick = .2f;
    public AnimationCurve fallingObstacleRatio;
}
