using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



public class RoomGenerator : MonoBehaviour
{
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

    public const string ROOM_DATA_PATH = "Assets/Scripts/LE_CORL/Datas/Level4RoomDatas";

    public Transform roomTriggerBundle;
    public Transform pointA, pointB;

    public int roomDuration = 30;
    public int fallingObstaclesCountMax = 10;
    public float fallingObstaclesMinDistancing = 1;
    public float fallingObstclesMinTimeGap = .2f;
    public AnimationCurve fallingObstacleRatio;

    private void Awake()
    {
#if UNITY_EDITOR

#else
        Destroy(gameObject);
#endif
    }
}
