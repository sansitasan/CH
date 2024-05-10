using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class MemoryPoolManager : MonoBehaviour, ICore
{
    [SerializeField] int GenerationDefaultCount = 15;

    private Dictionary<string, List<GameObject>> pools;
    private Dictionary<string, GameObject> originals;

    public bool IsIntitalized => isInited;
    bool isInited;

    public void Init()
    {
        pools = new Dictionary<string, List<GameObject>>();
        originals = new Dictionary<string, GameObject>();
        isInited = true;
    }

    
    
    void GeneratePoolObjects(string objName, GameObject origianl = null)
    {
        if (!pools.ContainsKey(objName))
        {
            Debug.LogError("등록되지 않은 오브젝트입니다");
            return;
        }

        var pool = pools[objName];
        var item = origianl != null ? origianl : originals[objName];

        float increaseRatio = (11 / 9f);
        int generationCount = pool.Count == 0 ? GenerationDefaultCount 
            : Mathf.RoundToInt((float)pool.Count * increaseRatio);

        for (int i = 0; i < generationCount; i++)
        {
            var go = Instantiate(item, transform);
            pool.Add(go);
            go.gameObject.SetActive(false);
        }
    }

    public static void RegisterMemorypoolObj(string objName, GameObject original)
    {
        var self = GameMainContoller.GetCore<MemoryPoolManager>();
        if (self.pools.ContainsKey(objName))
        {
            Debug.LogError("이미 등록된 오브젝트 입니다");
            return;
        }
        self.originals.Add(objName, original);
        self.pools.Add(objName, new List<GameObject>());
        self.GeneratePoolObjects(objName, original);
    }


    public static GameObject GetGameObject(string objName)
    {
        var self = GameMainContoller.GetCore<MemoryPoolManager>();
        if (!self.pools.ContainsKey(objName))
        {
            Debug.LogError("등록되지 않은 오브젝트입니다");
            return null;
        }
        var pool = self.pools[objName];
        var go = pool.Find((item) => !item.activeSelf);

        if (go == null)
        {
            self.GeneratePoolObjects(objName);
            return GetGameObject(objName);
        }
        else
        {
            return go;
        }
    }

    public static void ReturnPooledObjectTransform(Transform tf)
    {
        tf.gameObject.SetActive(false);
        var self = GameMainContoller.GetCore<MemoryPoolManager>();
        tf.SetParent(self.transform);
    }

    public static void UnregisterMemoerypool(string objName)
    {
        var self = GameMainContoller.GetCore<MemoryPoolManager>();
        if (!self.pools.ContainsKey(objName))
        {
            Debug.LogError("등록되지 않은 오브젝트입니다");
            return;
        }
        self.DeleteMemoerypool(objName).Forget();
        /*
#if UNITY_EDITOR
        self.DestroyMemorypoolImmediatly(objName);
#else
#endif
        */
    }

    void DestroyMemorypoolImmediatly(string objName)
    {
        var pool = pools[objName];

        foreach (var item in pool)
        {
            Destroy(item.gameObject);
        }
        pool.Clear();
        pools.Remove(objName);
        // Destroy(originals[objName]);
        originals.Remove(objName);
    }

    async UniTaskVoid DeleteMemoerypool(string objName)
    {
       // var pool = pools[objName];

        if(!pools.Remove(objName, out var destroyList))
        {
            Debug.LogError($"{objName} does not exist");
            return;
        }
        originals.Remove(objName);

        foreach (var item in destroyList)
            Destroy(item.gameObject);
        await UniTask.WaitUntil(() => destroyList.Count != 0);
        destroyList = null;
        await UniTask.Yield();

        print("memorypool - delete");
    }


    void DeleteMemoryPool(string objectName)
    {
        var pool = pools[objectName];
        foreach (var item in pool)
        {
            Destroy(item.gameObject);
        }
        pool.Clear();
        pools.Remove(objectName);
    }

    [ContextMenu("Editor: Print Pool List")]
    void Edit_PrintPoolList()
    {
        print($"총 개수: {pools.Count}");
        foreach (var pool in pools)
            print(pool.Key);

        print($"총 오리지널 개수: {originals.Count}");
        foreach (var item in originals)
            print(item.Key);
    }

    public void Disable()
    {
        var list = pools.Keys;
        foreach (var item in list)
            DeleteMemoryPool(item);

        pools.Clear();
        originals.Clear();
    }
}

/// 코인 관련
///     서로 다른 2종류 코인 존재
///        금: 플레이어 행동에 따른 권장 경로
///        은: 일반적인 길
///        
/// 맵 관련
///     맵 높낮이 없음 => 고군분투
///     
/// 
/// 
/// 맵 관련
///     맵에 높낮이가 있는가?
/// 
///     생성 메커니즘에 관하여
///     정의:
///         청크(가칭):
///             맵을 x 기준 3 ~ 5 정도의 크기로 자른 단위
///         생성 메커니즘:
///             한 청크가 생성되고 다음에 생성될 청크를 결정하는 일련의 과정
///             ex)     함정 청크 사이의 거리는 최소 n청크 이상
///                     코인이 n 주로 생성됨 등
///     옵션:
///         옵1. 사전에 완전히 정의된 청크를 일정한 규칙(일부 랜덤한 메커니즘)에 따라 생성
///             청크에 맵 오브젝트와 코인을 완전히 배치해 둠
///             생성 규칙 일부를 에디터에서 컨트롤 가능하게 함
///                 청크 생성 순서, 빈도 등
///             
///         옵2. 사전에 일부 정의된 청크를 일정한 규칙에 따라 생성
///             청크에 맵 오브젝트만 배치, 코인은 생성 메커니즘에 포함
///             생성 규칙 일부를 에디터에서 컨트롤 가능하게 함
///                 청크 생성 순서, 빈도,
///                 코인 생성 규칙
///         
///         옵3. 사전에 정의된 청크가 없음
///             청크의 개념만 정의하고 실제로 청크를 만들어 두지 않음
///                 청크 자체가 런타임 시 실시간으로 조립됨. => 마크 맵 생성과 유사한 방식
///             생성 규칙 전부를 에디터에서 컨트롤 가능하게 함
///                 청크 크기, 생성 확률, 생성 순서, 빈도
///                 코인 생성 규칙
///                 목표 청크(플레이어가 도달하도록 하는 청크)
///                 
/// 
///         부록:
///             청크 예시                               코인 예시
///                 직선,                                     직선,
///                 오르막,                                   높은 직선,
///                 내리막,                                   위-아래 2라인
///                 함정1 = 짧은 구멍                          3라인
///                 함정2 = 긴 구멍                            위로 볼록
///                 함정3 = 훅                                 아래로 볼록
///                 ...                                         ...