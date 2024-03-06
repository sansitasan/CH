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

/// 스테이지 4 관련 개발노트: 
///     표시: 4Stage - b
///     업로드: 24.03.06 2:10
///     
/// ---------
/// (Stage4 Scene 내부에 작성된 내용입니다)
/// 
/// 플레이어 관련
///     타비 캐릭터 
///         -이동, 에니메이션, 적용
///         Player 오브젝트 선택 -> 인스펙터에서 PlayerContolIsomatric 컴포넌트
///             vCam = 플레이어를 따라다니는 카메라
///             playerRenderer = 타비 캐릭터 
///             
///             Move Speed = 캐릭터 움직임 속도
///             CamFov = 플레이어 카메라 화면 넓이
///             skill = 스테이지에서 사용할 스킬
///             
///     스킬 일부 구현
///         스킬 사용시 화면 효과 구현
///         스킬 관련값 수정 방법:
///             (플레이어 컴포넌트 -> 스킬 더블클릭
///             or "Assets/Scripts/LE_CORL/Player/PlayerDatas/" 경로)
///         관련 값
///             Skill Cooldown = 스킬 쿨타임
///             Target Color = 스킬이 발동되었을 때 playerRenderer 가 변하게 되는 색상
///             Duration = 스킬 지속시간
///         스킬 로직
///             스킬 사용 -> 사용 종료 -> 쿨다운
///         
///     
/// 레벨(스테이지) 관련
///     스테이지 메인 컨트롤러 
///         : 씬에 존재하는 모든 트리거 관리
///         Level4Core 오브젝트 선택
///         
///     방 생성기
///         방: 낙하 장애물이 떨어지는 공간
///         Level4Core 오브젝트 선택 (펼치기) -> Room Generator 선택
///         : 실제 런타임에서는 스스로 파괴되는 오브젝트
///         생성된 데이터는 지정된 경로에 저장됨
///         지정된 경로: "Assets/Scripts/LE_CORL/Datas/Level4RoomDatas/" 
///             (요청시 변경 가능: RoomGenerator 스크립트 확인)
///     
///     타이머
///         방 - 타이머 연동
///         Level4Core 오브젝트 선택 (펼치기) -> Stage Canvas -> Timer 선택
///         
/// 이벤트 확인
///     플레이 모드에서 유니티 콘솔에 다음과 같은 메시지 출력
///         - player trigger enter: (물체 이름)
///             :(물체 이름)이 플레이어에 닿은 경우 출력
///         - player trigger extir: (물체 이름)
///             :(물체 이름)이 플레이어에서 떨어진 경우 출력
///         - Game Over
///             :플레이어가 낙하 장애물에 닿았을 때 추가로 출력
///         
