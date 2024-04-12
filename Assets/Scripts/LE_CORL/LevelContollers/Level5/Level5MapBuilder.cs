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


    Queue<int> shuffeldChunkIndexQueue;
    Queue<Level5MapChunk> loadedChunks;
    bool[] isTrapChunkIndex;
    int generatedChunkCounter;
    int lastTrap;

    Transform player;
    bool onRun;


    private void Start()
    {
        UpdateMapRuleset_Editor();
        Level5PlayerController.OnPlayStateChanged += Level5PlayerController_OnPlayStateChanged;
    }

    private void Level5PlayerController_OnPlayStateChanged(object sender, Level5PlayerController.PlayerStateChangeEventArgs e)
    {
        player = player != null ? player : ((Level5PlayerController)sender).transform;
        onRun = e.onRunning;

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
        InitMapHolder();

        GenerateEmptyChunk(currentMapRuleset.emptyChunkLenght);

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
        GenerateChunks(-1);
    }

    /// <summary>
    /// 청크 여러개를 한번에 생성하는 함수
    /// </summary>
    void GenerateChunks(int count)
    {
        count = count <= 0 ? generationCount : count;
        for (int i = 0; i < count; i++)
        {
            GenerateChunk();
        }
    }

    void InitMapHolder()
    {
        if (currentMapRuleset == null)
        {
            Debug.LogError("Level5: 맵 룰셋이 없습니다");
            return;
        }

        if (holder != null)
            Destroy(holder.gameObject);
        holder = new GameObject("holder").transform;
        holder.parent = transform;
        // holder.gameObject.AddComponent <Level5MapController> ();

        loadedChunks = new Queue<Level5MapChunk>();
        generatedChunkCounter = 0;
        lastTrap = 0;
    }

    void GenerateEmptyChunk(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GenerateChunk(0);
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
        newChunk.localPosition = new Vector2(generatedChunkCounter * level5ChunkSettingData.chunkSizeX + chunksUnloadPositionX, 0);

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
                GenerateChunks(-1);
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

    [ContextMenu("Editor: Generate Empty Chunk")]
    public void GenerateEmptyChunk_Editor()
    {
        var pos = new Vector2(20, 20);
        for (int i = 0; i < currentMapRuleset.emptyChunkLenght; i++)
        {
            var newChunk = Instantiate(currentMapRuleset.chuckPrefabs[0], pos, Quaternion.identity);
            newChunk.parent = holder;
            newChunk.localPosition = new Vector2(generatedChunkCounter * level5ChunkSettingData.chunkSizeX + chunksUnloadPositionX, 0);
            generatedChunkCounter++;
        }
    }


}


/// 240410: 5스테 기능 업데이트 level5 func update
/// 
/// 맵 관련
///     맵 빌더: 설정 값을 읽어와 맵 오브젝트를 생성하는 기능
///     오브젝트: 하이라키 / Level5 / Map Bundle / 인스펙터 / Level5MapBuilder 스크립트
///     주요 기능:
///         1. 관련 데이터를 저장
///         2. 맵 오브젝트 생성, 유지, 파괴
///     변수:
///         Level5 Chunk Setting Data: 모든 청크에 적용하는 범용 규칙
///             청크의 가로 폭, 코인의 크기와 배치를 설정
///         Holder: 생성된 맵 오브젝트의 부모 오브젝트
///         Chunks Unload Position X: 플레이어 위치를 기준으로 청크를 파괴하는 거리
///         Current Map Ruleset: 현재 적용중인 맵 설정
///             사용되는 청크의 프리팹, 맵 시드, 함정 청크 간의 거리 등을 설정
///         Generation Count: 맵 생성시 기본으로 생성되는 청크의 개수
///         Maintain Count: 맵에 유지되는 청크의 최소 개수
///         Coin Prefab: 코인 프리팹
///         
///     청크 범용 규칙: Level 5 Chunk Settings
///         참조: Assets\Scripts\LE_CORL\Datas\Level5\Level 5 Chunk Settings
///         Chunk Size X: 단일 청크의 x 크기, (자동 설정x, 맵 생성에만 영향을 줌)
///         Coin Pivot: 각 청크에 코인을 배치할 때 기준점이 되는 좌표
///         Coin Distancing: 각 청크에 코인을 배치할 때 코인 사이의 거리
///         Coin Heigth Start: 코인을 배치하는 시작 y 좌표
///         Coin Heigth Limit: 코인을 배치하는 최대 y 좌표
///         
///     맵 설정: Level 5 Map Generation Ruleset
///         참조: Assets\Scripts\LE_CORL\Datas\Level 5 Map Generation Ruleset
///         Chunk Prefabs: 사용할 청크 프리팹 리스트
///         Seed: 맵 생성에 사용되는 시드값. 사전에 지정 가능(같은 조건이라면 항상 같은 맵을 만듦)
///         Empty Chunk Lenght: 맵의 시작 부분에 생성되는 '빈 청크'의 길이
///         Trap Distancing: '함정 청크' 간의 거리
///         
///     청크
///         맵을 구성하는 최소 단위
///         참조: Assets\Prefabs\Stage5\Chunks\
///         청크 타입: 해당 청크의 타입: 베이스/Empty/Ground/Trap 이 있으며, 더 추가 가능
///         생성 빈도: 해당 청크가 생성되는 빈도(상대값)
///         코인 정렬하기: 해당 청크(프리팹)의 코인을 범용 규칙에 맞게 정렬
///     
/// 
/// 플레이어 관련
///     입력: 
///         스페이스 바 입력 만 받음. 플레이어의 상태에 따라 다른 기능을 출력함
///             기본(땅에 있을 때): 점프
///             점프(공중에 있을 때): 훅 발사
///             훅이 걸려 있을 때: 스페이스 바에서 손을 떼면 앞으로 밀려나며 훅 해제
///     관련 변수:
///         Jump Force: 점프 계수
///         Move Speed: x축 움직임 속도
///         On Run: x축 움직임 여부
/// 
///     테스트 입력:
///         실제 런타임이 아닌 테스트를 위한 입력:
///             m 키: 달리기 시작/종료
/// 
///     훅 시스템:
///         하이라키/Level5/Hook System
///         Throwing Duration: 훅 판정까지 최대 시간
///         Throwing MAx Distance 훅을 던졌을 때 최대 도달 거리 (정확하게 45도 각도로 날아감)
///         Hook Duration: 훅이 걸렸을 때 유지되는 최대 시간
/// 
/// 
/// 새로운 청크 만드는 방법:
///     1. 청크 프리팹 위치로 이동: Assets\Prefabs\Stage5\Chunks\
///     2. (ex)ChunkBase 오브젝트 선택
///     3. 같은 경로에 복사-붙여넣기
///     4. 새로 만들어진 오브젝트의 이름을 바꾸고 더블클릭
///     5. 에디터의 씬 뷰에서 배치 편집
///         5-1. 코인은 위치만 잡으면 런타임에서 따로 생성됩니다
///         5-2. 코인을 사용하지 않을 경우, 해당 position을 지우셔야 합니다. 
///             부모 오브젝트(----되어 있는 거)는 지우시면 안됩니다.
///         5-3. 꼭 '코인 정렬'을 할 필요는 없습니다. 반대로 대충 러프하게만 둬도 '코인 정렬'을 누르면 정확한 위치로 배치됩니다.
///     6. 인스펙터에서 청크 타입, 생성 빈도 설정
///         6-1. 청크 타입은 Base 이외를 선택해야 합니다. (Base는 새로운 청크를 만드는 용도)
///     7. 프리팹 저장
///     8. 씬으로 돌아와 하이라키 / Level5 / Map Bundle / 인스펙터 / Level 5 Map Generation Ruleset / 더블클릭
///         혹은 Assets\Scripts\LE_CORL\Datas\Level 5 Map Generation Ruleset 선택
///     9. 인스펙터 / "청크 불러오기" 버튼 클릭 
///         -> 청크가 정상적으로 불러와졌는지 확인
///         