using Cysharp.Threading.Tasks.Triggers;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

//[CreateAssetMenu(fileName = "new Room (Level4)", menuName = "Datas/Level4 Room Ruleset", order = 0)]
public class Level4RoomRuleset : ScriptableObject
{
    public Vector2 pointA, pointB;
    public int roomDuration = 30;
    public int generationMax = 10;
    public float minDistancing = 1;
    public float generationTick = .2f;
    public AnimationCurve generationRatio;
    [Range(0, 100)] public int[] generationPrio = new int[8];
    public int shuffleSeed;
    public bool isEscapeableRoom;

    public Queue<int> GetRandomizedFallingObjectQueue()
    {
        List<int> list = new List<int>();

        for (int i = 0; i < generationPrio.Length; i++)
        {
            int targetObjectIDX = i;
            int count = generationPrio[i];
            for (int j = 0; j < count; j++)
                list.Add(targetObjectIDX);
        }
        list = Util.ShuffleList(list, shuffleSeed);
        Queue<int> result = new Queue<int>();
        list.ForEach(x => result.Enqueue(x));

        if(result.Count <= 0)
        {
            Debug.LogError($"장애물 우선순위 미설정: {this.name}");
        }

        return result;
    }
}
