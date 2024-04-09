using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static Unity.Burst.Intrinsics.X86.Avx;

public class Level5MapBuilder : MonoBehaviour
{
    public const string CHUNK_RULESET_PATH = "Assets/Scripts/LE_CORL/Datas/Level5";
    public const string COIN_OBJECTPOOLING_KEY = "level5_coin";

    [Header("+ Chuck")]
    public Level5ChunkSettings level5ChunkSettingData;
    public Transform holder;
    public float chunksUnloadPositionX = -20;
    
    [Header("+ Map")]
    public Level5MapGenerationRuleset currentMapRuleset;
    [Min(12)] public int generationCount;
    [Min(6)] public int maintainCount;

    [Header("+ Coin Prefab")]
    public GameObject coinPrefab;

    [Range(1, 5)] public float mapSpeedBase;


    Queue<int> shuffeldChunkIndexQueue;
    Queue<Level5MapChunk> loadedChunks;
    bool[] isTrapChunkIndex;
    int generatedChunkCounter;
    int lastTrap;

    Transform player;
    bool onRun;
    float playerSpeed;


    private void Start()
    {
        UpdateMapRuleset_Editor();
        Level5PlayerController.OnPlayStateChanged += Level5PlayerController_OnPlayStateChanged;
    }

    private void Level5PlayerController_OnPlayStateChanged(object sender, Level5PlayerController.PlayerStateChangeEventArgs e)
    {
        player = player != null ? player : ((Level5PlayerController)sender).transform;
        onRun = e.onRunning;
        playerSpeed = e.speed;

        if (onRun)
            TrackingPlayerPosition().Forget();
    }


    /// <summary>
    /// 맵을 바꾸는 함수
    /// </summary>
    /// <param name="ruleset"></param>
    public void UpdateMapRuleset(Level5MapGenerationRuleset ruleset)
    {
        currentMapRuleset = ruleset;
        GenerateNewMap();
    }

    #region 맵 생성


    /// <summary>
    /// 맵 전체를 생성하기 위한 로직
    /// </summary>
    void GenerateNewMap()
    {
        MemoryPoolManager.RegisterMemorypoolObj(COIN_OBJECTPOOLING_KEY, coinPrefab);

        if (currentMapRuleset == null)
        {
            Debug.LogError("Level5: 맵 룰셋이 없습니다");
            return;
        }

        if(holder != null)
            Destroy(holder.gameObject);
        holder = new GameObject("holder").transform;
        holder.parent = transform;
        // holder.gameObject.AddComponent <Level5MapController> ();

        loadedChunks = new Queue<Level5MapChunk>();
        generatedChunkCounter = 0;
        lastTrap = 0;   

        // 시작지점(빈 땅) 생성
        for (int i = 0; i < currentMapRuleset.emptyChunkLenght; i++)
        {
            GenerateChunk(0);
        }

        // 맵 생성 큐 생성
        List<int> prio = new List<int>();
        int prefabCount = currentMapRuleset.chuckPrefabs.Count;
        isTrapChunkIndex = new bool[prefabCount];
        for (int i = 0; i < prefabCount; i++)
        {
            var chunkData = currentMapRuleset.chuckPrefabs[i].GetComponent<Level5MapChunk>();
            isTrapChunkIndex[i] = chunkData.IsTrapChunk();
            for (int c = 0; c < chunkData.priority; c++)
                prio.Add(i);
        }
        var shuffle = Util.ShuffleList(prio, currentMapRuleset.seed);
        shuffeldChunkIndexQueue = new Queue<int>();
        foreach (var item in shuffle)
            shuffeldChunkIndexQueue.Enqueue(item);


        // 맵 생성
        GenerateChunks();
    }

    /// <summary>
    /// 청크 여러개를 한번에 생성하는 함수
    /// </summary>
    void GenerateChunks()
    {
        for (int i = 0; i < generationCount; i++)
        {
            GenerateChunk();
        }
    }

    /// <summary>
    /// 청크 하나를 생성하는 함수
    /// </summary>
    void GenerateChunk()
    {
        var idx = GetNextGenerationIndex();
        if(generatedChunkCounter < currentMapRuleset.trapDistancing + lastTrap)
        {
            while (isTrapChunkIndex[idx])
                idx = GetNextGenerationIndex();
        }

        GenerateChunk(idx);
    }

    /// <summary>
    /// 청크 하나를 생성하는 함수: 생성할 청크를 특정할 수 있음
    /// </summary>
    void GenerateChunk(int targetIDX)
    {
        var pos = new Vector2(20, 20);
        var newChunk = Instantiate(currentMapRuleset.chuckPrefabs[targetIDX], pos, Quaternion.identity);

        newChunk.parent = holder;
        newChunk.localPosition = new Vector2(generatedChunkCounter * level5ChunkSettingData.chunkSizeX, 0);

        var chunk = newChunk.GetComponent<Level5MapChunk>();

        loadedChunks.Enqueue(chunk);

        generatedChunkCounter++;
        lastTrap = chunk.IsTrapChunk() ? generatedChunkCounter : lastTrap;
    }

    /// <summary>
    /// 다음 생성될 청크의 인덱스를 받아오는 함수
    /// </summary>
    /// <returns></returns>
    int GetNextGenerationIndex()
    {
        var idx = shuffeldChunkIndexQueue.Dequeue();
        shuffeldChunkIndexQueue.Enqueue(idx);
        return idx;
    }
    #endregion


    #region 청크 관리

    async UniTaskVoid TrackingPlayerPosition()
    {
        float timeGap = .5f;
        float disableDistance = 15f;
        while (onRun)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(timeGap));

            var tmp = loadedChunks.Peek();
            while (tmp.transform.position.x < player.position.x - disableDistance)
            {
                var deque = loadedChunks.Dequeue();
                deque.DestroySelf();
                tmp = loadedChunks.Peek();
            }

            if(loadedChunks.Count <= maintainCount)
            {
                GenerateChunks();
            }
        }
    }

    #endregion





    [ContextMenu("Editor: Update MapRuleset")]
    public void UpdateMapRuleset_Editor()
    {
        UpdateMapRuleset(currentMapRuleset);
    }

    [ContextMenu("Editor: Test Generation")]
    public void TestGeneration_Editor()
    {
        GenerateNewMap();
    }

}


/// todo
/// 청크 => 알아서 움직이는 로직
/// 맵 빌더 =>
///     맵 생성 관련    
///         최초 -> n개의 청크를 생성
///         이후 -> 청크의 개수를 n개로 유지하는 로직
///     맵 이동 관련 관련
///         맨 앞의 청크의 x 좌표만 확인 (움직임은 알아서)
///             맨 앞 청크의 x 좌표가 특정값 이하로 떨어지면 해당 청크와 엔티티를 제거
