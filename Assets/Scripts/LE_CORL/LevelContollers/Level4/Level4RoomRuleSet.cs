using System.Collections;
using UnityEngine;

namespace Assets.Scripts.LE_CORL.LevelContollers.Level4
{
    public class Level4RoomRuleSet : ScriptableObject
    {
        public Vector2 pointEnter, pointA, pointB, pointExit;

        public int roomDuration = 30;
        public int fallingObstaclesCountMax = 10;
        public float fallingObstaclesMinDistancing = 1;
        public AnimationCurve fallingObstacleRatio;
    }
}