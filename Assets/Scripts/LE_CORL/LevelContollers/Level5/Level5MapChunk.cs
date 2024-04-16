using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class Level5MapChunk : MonoBehaviour
{
    public enum ChunkType
    {
        Base,
        Empty,
        Ground,
        Trap,
    }

    [Header("+ Chuck Setting")]
    public ChunkType myChuckType;
    [Range(0, 100)]
    public int priority;

    [Header("+ Bundles")]
    public Transform silverCoins;
    public Transform goldCoins;

    List<Transform> myCoins;

    private void OnEnable()
    {
        if (myChuckType == ChunkType.Base)
        {
            Debug.LogError("잘못된 청크 설정: 청크 타입을 Base가 아닌 다른 것으로 선택해 주세요");
            Destroy(gameObject);
        }

        // 코인 생성
        myCoins = new List<Transform>();
        if (silverCoins != null)
            foreach (Transform t in silverCoins)
            {
                var go = MemoryPoolManager.GetGameObject(Level5MapBuilder.COIN_OBJECTPOOLING_KEY);
                go.transform.position = t.position;
                go.transform.parent = transform;
                myCoins.Add(go.transform);

                go.GetComponent<Level5Coin>()?.SetCoinType(Level5Coin.CoinType.Silver);

                go.SetActive(true);
            }
        if (goldCoins != null)
            foreach (Transform t in goldCoins)
            {
                var go = MemoryPoolManager.GetGameObject(Level5MapBuilder.COIN_OBJECTPOOLING_KEY);
                go.transform.position = t.position;
                go.transform.parent = transform;
                myCoins.Add(go.transform);

                go.GetComponent<Level5Coin>()?.SetCoinType(Level5Coin.CoinType.Gold);

                go.SetActive(true);
            }
    }

    public void DestroySelf()
    {
        foreach (var item in myCoins)
        {
            MemoryPoolManager.ReturnPooledObjectTransform(item);
        }
        myCoins.Clear();
        Destroy(gameObject);
    }

    public bool IsTrapChunk()
    {
        return this.myChuckType == ChunkType.Trap;
    }


    List<Transform> GetAllCoinPositions()
    {
        var result = new List<Transform>();
        if (silverCoins != null)
            foreach (Transform t in silverCoins)
                result.Add(t);
        if (goldCoins != null)
            foreach (Transform t in goldCoins)
                result.Add(t);

        return result;
    }


    #region Editor

    [ContextMenu("Sort Coins")]
    public void SortCoins()
    {
        var path = Path.Combine(Level5MapBuilder.CHUNK_RULESET_PATH, "Level 5 Chunk Settings.asset");
        var ruleset = AssetDatabase.LoadAssetAtPath<Level5ChunkSettings>(path);
        var positionables = ruleset.GetCoinPositions();

        var myPositions = new List<Transform>();
        foreach (Transform t in silverCoins)
            myPositions.Add(t);
        foreach (Transform t in goldCoins)
            myPositions.Add(t);

        if(myPositions.Count > positionables.Count)
        {
            Debug.LogError($"FROM: 청크 {gameObject.name} \n" +
                " 코인의 개수가 너무 많거나, 코인을 위치 시킬 수 있는 범위가 너무 작습니다.");
        }

        bool[] alreadySet = new bool[positionables.Count];

        foreach (var coinPos in myPositions)
        {
            int minDistanceIDX = -1;
            float minDistance = ruleset.chunkSizeX;

            for (int distancingIDX = 0; distancingIDX < positionables.Count; distancingIDX++)
            {
                if (alreadySet[distancingIDX])
                    continue;
                var compareDis = Vector2.Distance(coinPos.localPosition, positionables[distancingIDX]);
                if (compareDis <= minDistance)
                {
                    minDistanceIDX = distancingIDX;
                    minDistance = compareDis;
                }
            }
            coinPos.localPosition = positionables[minDistanceIDX];
            alreadySet[minDistanceIDX] = true;
            // print($"{coinPos.name} to position {positionables[minDistanceIDX]}");
        }
    }
    #endregion

}
