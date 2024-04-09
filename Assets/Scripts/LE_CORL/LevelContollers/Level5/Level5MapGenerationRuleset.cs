using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level5MapGenerationRuleset : ScriptableObject
{   
    public List<Transform> chuckPrefabs;
    [Space(10)]

    public int seed;

    [Range(3, 10)] public int emptyChunkLenght = 5;
    [Min(2)] public int trapDistancing;
}