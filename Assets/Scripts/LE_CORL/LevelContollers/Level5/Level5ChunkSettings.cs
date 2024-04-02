using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class Level5ChunkSettings : ScriptableObject
{
    [Header("+ Chuck")]
    public float chunkSizeX = 5;

    [Header("+ Coins")]
    public Vector2 coinPivot = new Vector2(.5f, .5f);
    public Vector2 coinDistancing = new Vector2(1, 1);
    public float coinHeightStart = 0;
    public float coinHeightLimit = 3f;

    public List<Vector2> GetCoinPositions()
    {
        List<Vector2> positionalbe = new List<Vector2>();
        var fixedPivot = new Vector2(-chunkSizeX / 2f, 0) + coinPivot + Vector2.one * coinHeightStart;

        string posStr = "Coin Positions: \n";
        for (float x = fixedPivot.x; x < chunkSizeX / 2f; x += coinDistancing.x)
        {
            string line = " ";
            for (float y = fixedPivot.y; y < coinHeightLimit; y += coinDistancing.y)
            {
                var pos = new Vector2(x, y);
                positionalbe.Add(pos);
                line += " / " + pos;
            }
            line += "\n";
            posStr += line;
        }
        Debug.Log(posStr);
        return positionalbe;
    }


}
