using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class MemoryPoolManager : MonoBehaviour
{
    [SerializeField] int GenerationDefaultCount = 15;

    private Dictionary<string, List<GameObject>> pools;
    private Dictionary<string, GameObject> originals;

    public static MemoryPoolManager Instance { get; private set; } = null;


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            pools = new Dictionary<string, List<GameObject>>();
            originals = new Dictionary<string, GameObject>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

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



    public void RegisterMemorypoolObj(string objName, GameObject original)
    {
        if (pools.ContainsKey(objName))
        {
            Debug.LogError("이미 등록된 오브젝트 입니다");
            return;
        }
        originals.Add(objName, original);
        pools.Add(objName, new List<GameObject>());
        GeneratePoolObjects(objName, original); 
    }

    public GameObject GetGameObject(string objName)
    {
        if(!pools.ContainsKey(objName))
        {
            Debug.LogError("등록되지 않은 오브젝트입니다");
            return null;
        }
        var pool = pools[objName];
        var go = pool.Find((item) => !item.activeSelf);

        if (go == null)
        {
            GeneratePoolObjects(objName);
            return GetGameObject(objName);
        }
        else
        {
            return go;
        }
    }

    public async void UnregisterMemoryPool(string objName)
    {
        if (!pools.ContainsKey(objName))
        {
            Debug.LogError("등록되지 않은 오브젝트입니다");
            return;
        }

        await Task.Run(() => DeleteMemoryPool(objName));
        await Task.Run(() => 
        {
            Destroy(originals[objName]);
            originals.Remove(objName);
        });
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
}

/// 플레이어 관련
/// WASD 만 구현
/// 
/// -------
/// 
/// 방 생성 관련
/// Stage4Scene 진입 -> Level4Core 오브젝트 선택 (펼치기) -> Room Generator 선택
/// Room Generator 하위 목록에 PointA, PointB를 Scene 에서 위치 수정
/// 
/// Inspector 창을 보면 방 생성기 확인 가능
/// 생성 버튼을 누르면 다음과 같이 진행
///     pointA - pointB를 양 모서리로 사각형을 자동으로 생성함
///     project 창에 방 데이터 생성, 방 데이터 하이라이트
/// 데이터 파일 저장 위치
///     Assets/Scripts/LE_CORL/Datas/
/// 데이터 적용 위치 
///     Level4Core.Level4MainController => Datas / Room Rulesets
///     Level4Core 오브젝트 (펼치기) -> Event Triggers (펼치기) -> Rooms (펼치기)
///     하위에 오브젝트 생성(플레이어 트리거)
/// 
///     pointA, pointB = 방 범위
///     room Duration = 방 지속시간
///     falling Obstacles Count Max = 한 번에 생성되는 최대 개수
///     falling Obstacles Min Distance = 한 번에 생성되는 낙하장애물 간의 최소 거리
///     falling Obstacles Generation Tick = 생성을 시도학 틱
///     falling Obstacle Ratio = 진행도 대비 생성되는 낙하 장애물의 비율
/// 
/// 생성 공식
///     진행도 대비 생성 비율 * 최대 생성 개수
/// 
/// ! 주의사항 !
/// Level4Core 오브젝트의 Level4MainController에
///     Trigger 항목과 RoomRulesets 항목이 서로 다를 경우 에러 발생 가능 
///     -> (추후 수정할 예정. RoomRuleset의 수치를 테스트 하는 용도로 사용)
///
/// --------
/// 
/// 낙하 오브젝트 설정
/// Assets/Prefabs/Stage4/Falling Obstacle Prefab 오브젝트
/// 


/// 스테이지 4 관련 개발노트: 24.03.06 2:00 업로드
/// 
/// 