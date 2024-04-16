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



/// 240409 level4 falling object, room ctrl
/// 4스테 시스템 변경, 낙하 장애물 등
/// 
/// 낙하 장애물 관련
///     낙하 장애물 구현 완료
///         프리팹 위치: 
///             C:\Projects\CH\Project_CH_Git\CH\Assets\Prefabs\Stage4
///         각 낙하 장애물 관련 데이터 위치:
///             Assets\Scripts\LE_CORL\Datas\Level4\FalllingObejcts
///             
///     낙하 장애물 설정값
///         Mark_Duration: 떨어질 위치 표시 지속시간
///         Mark_Color: 떨어질 위치 표시할 때의 색상
///         Falling: 떨어지는 과정에서의 스프라이트 애니메이션 (순서대로 출력)
///         Falling_StartHeigth: 떨어지기 시작하는 높이(y축)
///         Fallling_Speed: 떨어지는 속도(시간-y위치 그래프)
///         FallingTriggerCheckingAmount: 떨어지는 과정 중 판정 비율 (떨어지는 시간 대비)
///         Falling_Duration: 떨어지는 총 시간
///         Falled: 떨어지는 과정에서의 스프라이트 애니메이션 (순서대로 출력)
///         Falled_DisableAnimDuration: 사라지는 애니메이션이 동작하는 시간
///         
///     
///     
/// 방 시스템 관련
///     기존 보다 직관적인 시스템으로 변경 -> 각 방의 트리거에서 직접 방 데이터를 관리
///     더 이상 메인 컨트롤러에서 관리하지 않음
///         
///     
/// 방 데이터 관련
///     데이터 파일 위치
///         Assets\Scripts\LE_CORL\Datas\Level4\Rooms
///     데이터 파일
///         Point A, B : 방의 대각선 방향 꼭짓점
///         RoomDuration: 방 지속시간
///         GenerationMax: 한 번에 생성되는 낙하장애물의 최대개수
///         Min Distancing: 한 번에 생성되는 낙하장애물 간의 최소 간격
///         Generation Tick: 낙하 장애물을 생성하는 주기
///         Generation Ratio: 낙하 장애물 생성 계수 (시간-개수 그래프)
///             생성 개수 = ratio(t) * GenerationMax
///         Generation Pro: 각 낙하 장애물 간의 생성 빈도
///         Shuffle Seed: 낙하 장애물의 생성 시드 (랜덤값)
///         IsEscapeableRoom: 방을 중간에 빠져나갈 수 있는지 여부
///         
/// 방 생성기 관련
///     하이라키 창, Level4Core / Room Generator
///         방 시스템 변화에 맞게 수정
///         싱크 맞추기 기능 추가
///             지정된 경로의 데이터파일을 기준으로 씬에 오브젝트를 재생성
///             데이터 파일의 넘버링에 이상이 있는 경우, 순서대로 정렬
///             
/// 버그 픽스
///     방 생성기 인스펙터가 동작하지 않던 버그
///     방 지속시간이 종료되었지만 낙하 장애물이 계속 생성되는 버그
///     방 지속시간이 종료되었지만 타이머가 사라지지 않던 버그
///     
///     
///     
///     