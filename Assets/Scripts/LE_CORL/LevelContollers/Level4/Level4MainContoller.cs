using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using Assets.Scripts.LE_CORL.Player;
using Cinemachine;
using Unity.VisualScripting;
using Cysharp.Threading.Tasks;
using UnityEngine.Timeline;

public class Level4MainContoller : LevelControllerBase
{
    public const string FALLING_OBJECT_ID = "falling_object_";

    public static Level4MainContoller Instance;

    [Header("Object Refer")]
    [SerializeField] List<PlayerEventTrigger> timelineTriggers;
    [SerializeField] Level4PlayerController player;

    [Header("Room")]
    [SerializeField] CinemachineBlenderSettings blendSetting;
    [SerializeField] float roomPostDelay;

    [Header("Gamm Play")]
    [SerializeField] GameObject invisibleWall;
    [SerializeField, Range(1,5)] int playerHPMax = 3;

    [Header("Timeline")]
    [SerializeField] PlayableDirector playerableDicetor;

    [Header("Prefabs")]
    [SerializeField] GameObject[] fallingObjectPrefabs;

    public float CinemachineBlendDuration
    {
        get 
        {
            if (blendSetting == null) return 0f;
            else 
                return blendSetting.m_CustomBlends[0].m_Blend.m_Time;
        }
    }

    public float RoomPostDelay => roomPostDelay;

    public static event EventHandler<PlayerHPUpdaterEventArgs> OnPlayerHPUpdated;
    public class PlayerHPUpdaterEventArgs : EventArgs
    {
        public int current;
        public int max;
        public bool isInit;
    }

    int playerHP;

    private void Level4RoomController_OnAnyRoomStateChanged(object sender, Level4RoomController.RoomStateChangedEventArgs e)
    {
        var room = (Level4RoomController)sender;
        if(e.isStarted)
        {
            //respawnPosition = ((Level4RoomController)sender).myRespawnPosition.position;
            room.myCamera.gameObject.SetActive(true);
            invisibleWall.SetActive(true);
            playerHP = playerHPMax;
            OnPlayerHPUpdated(this, new PlayerHPUpdaterEventArgs { current = playerHP, max = playerHPMax, isInit = true });;
        }
        else
        {
            invisibleWall.SetActive(false);
            if (!e.isCleared)
                FailToClear(room.myRespawnPosition.position, room.myCamera.gameObject).Forget();
            else
                room.myCamera.gameObject.SetActive(false);
        }
    }

    private void PlayerEventTrigger_OnPlayerEntered(object sender, EventArgs e)
    {
        PlayerEventTrigger trigger = (PlayerEventTrigger)sender;
        var id = trigger.EventID;
        // print($"player trigger enter: {id}");

        if (!id.StartsWith("level4_"))
            return;

        if (id.Contains(FALLING_OBJECT_ID))
            PlayerHit();
    }

    void PlayerHit()
    {
        if(playerHP <= 0) return;

        playerHP = player.PlayerDamaged(playerHP);
        OnPlayerHPUpdated(this, new PlayerHPUpdaterEventArgs { current = playerHP, max = playerHPMax });
    }

    async UniTaskVoid FailToClear(Vector2 respawnPos, GameObject vCam)
    {
        player.enabled = false;
        var animator = player.GetComponentInChildren<Animator>();
        await UniTask.WhenAll(
            UniTask.WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).length >= 1),
            UniTask.Delay(TimeSpan.FromSeconds(RoomPostDelay))
            );

        player.transform.position = respawnPos;
        vCam.SetActive(false);
        await UniTask.Delay(TimeSpan.FromSeconds(RoomPostDelay));

        animator.SetBool("OnDead", false);
        player.enabled = true;
    }


    void PlayTimeline(TimelineAsset timeline)
    {
        playerableDicetor.Play(timeline);
    }

    protected override void InitializeScene()
    {
        // 메모리풀 등록
        for (int i = 0; i < fallingObjectPrefabs.Length; i++)
        {
            var go = fallingObjectPrefabs[i];
            var id = FALLING_OBJECT_ID + i;
            MemoryPoolManager.RegisterMemorypoolObj(id, go);
        }
    }

    protected override void StartSceneWithoutInit()
    {
        Instance = this;

        Level4RoomController.OnAnyRoomStateChanged += Level4RoomController_OnAnyRoomStateChanged;
        PlayerEventTrigger.OnPlayerEntered += PlayerEventTrigger_OnPlayerEntered;
    }

    protected override void DiposeScene()
    {
        Level4RoomController.OnAnyRoomStateChanged -= Level4RoomController_OnAnyRoomStateChanged;
        PlayerEventTrigger.OnPlayerEntered -= PlayerEventTrigger_OnPlayerEntered;

        Instance = null;
    }

    protected override void ToLobbyScene()
    {
        // 메모리풀 삭제
        for (int i = 0; i < fallingObjectPrefabs.Length; i++)
        {
            var go = fallingObjectPrefabs[i];
            var id = FALLING_OBJECT_ID + i;
            MemoryPoolManager.UnregisterMemoerypool(id);
        }
    }
}

/// 240419. level4 dev
/// 게임 내용 관련
///     - 각 방 마다 카메라 추가 (각 방 trigger의 자식 오브젝트)
///     - 각 방 마다 리스폰 장소 추가 (각 방 trigger의 자식 오브젝트)
///     - 투명 벽 추가
///         투명 벽과 방 트리거 사이 공간이 남는 문제로 인해 방 트리거의 가로 크기가 0.5 정도 커짐
///     - 낙하 오브젝트의 생성 로직 일부 수정
///         더 이상 벽 위에 떨어지지 않음
///     - 플레이어 캐릭터 판정 기능 추가
///         낙하 오브젝트로 부터 발동
///         판정 후 무적시간 추가 (Level4Core 오브젝트의 인스펙터에서 수정 가능)
///         스킬 사용 시 무적 기능 적용 (무적시간과 관계 없음)
///     - 플레이어 캐릭터 HP 추가 (Level4Core 오브젝트의 인스펙터에서 수정 가능)
///     - 플레이어 캐릭터 HP UI 추가 (임시)
///     - 방이 종료된 후 방의 클리어 여부에 따라 다음 내용을 수행함
///         - 화살표 출력 (임시)
///         - 방의 리스폰 장소로 플레이어 이동 후 잠시 입력을 중지함
///         
/// 에디터 관련 변경사항
///     - RoomGenerator 기능 일부 수정
///         - "싱크 맞추기" 기능에 카메라, 리스폰 장소 생성 기능 추가
///         - "싱크 맞추기" 기능에 방 트리거 크기를 조정하는 내용 추가
///     - Level4Core.Blend Setting 에서 카메라 전환 관련 내용을 수정할 수 있음
///     - Level4Core.Player HP MAx 로 플레이어의 HP 최대치를 수정할 수 있음
///         각 방에 들어갈 때 max 로 초기화 됨
///     - Level4Core.Damageable Time Gap 으로 플레이어의 무적시간을 수정할 수 있음 (최솟값= 0.05)
///     
/// 기타 사항
///     다음 내용, 일부 로직 변경
///         - 방 진행 로직
///         - 게임 진행 트리거
///         - 게임 진행 UI 관련 트리거