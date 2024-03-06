using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using Assets.Scripts.LE_CORL.Player;

public class Level4MainContoller : MonoBehaviour
{
    [Header("Object Refer")]
    [SerializeField] List<PlayerEventTrigger> triggersInLevel;
    [SerializeField] List<Level4RoomRuleset> roomRulesets;

    [SerializeField] PlayerControlIsomatric player;

    [Header("Timeline")]
    [SerializeField] PlayableDirector playerableDicetor;
    [SerializeField] TimelinePreferences[] timelinePreferences;

    public Transform roomTriggersParent;

    public static event EventHandler<RoomStateChangedEventArgs> OnRoomStateChanged;
    public class RoomStateChangedEventArgs: EventArgs
    {
        public bool isStartRoom;
        public Level4RoomRuleset roomRulesetData;
    }
    public static event EventHandler OnGameOver;


    private void Awake()
    {
        
    }

    private void OnEnable()
    {
        PlayerEventTrigger.OnPlayerEntered += PlayerEventTrigger_OnPlayerEntered;
        PlayerEventTrigger.OnPlayerExited += PlayerEventTrigger_OnPlayerExited;

        triggersInLevel[0].SetTriggerActivation(true);
    }


    private void PlayerEventTrigger_OnPlayerEntered(object sender, System.EventArgs e)
    {
        PlayerEventTrigger trigger = (PlayerEventTrigger)sender;
        var id = trigger.EventID;
        print($"player trigger enter: {id}");

        if (!id.StartsWith("level4_"))
            return;

        if (id.Contains("room_"))
        {
            int num = int.Parse(id.Split("room_")[1]);
            OnRoomStateChanged?.Invoke(sender, new RoomStateChangedEventArgs
            {
                isStartRoom = true,
                roomRulesetData = roomRulesets[num]
            });
        }
        else if (id.Contains("timeline_"))
        {

        }
        else if (id.Contains("fallingObstacle_"))
        {
            if (IsGameOver())
                Debug.Log("GameOver");
        }
    }


    private void PlayerEventTrigger_OnPlayerExited(object sender, EventArgs e)
    {
        // 
        PlayerEventTrigger trigger = (PlayerEventTrigger)sender;
        var id = trigger.EventID;
        print($"player trigger exit: {id}");
    }

    public void AddNewRoomData(Level4RoomRuleset roomRuleset)
    {        if(roomRulesets == null)
            roomRuleset = new Level4RoomRuleset();

        roomRulesets.Add(roomRuleset);
    }

    [ContextMenu("Editor_SearchEventTriggersInChildren")]
    public void FindAllEventTriggersInChildren()
    {
        var searched = GetComponentsInChildren<PlayerEventTrigger>();
        foreach (var item in searched) 
        {
            if (triggersInLevel.Contains(item))
                continue;
            triggersInLevel.Add(item);
        }
    }



    bool IsGameOver()
    {
        if (player.skill.m_SkillState == PlayerSkillBase.SkillState.OnActiating)
            return false;
        else
            return true;
    }

#if UNITY_EDITOR
    public void UpdateEventTriggerList()
    {
        if(triggersInLevel == null)
            triggersInLevel = new List<PlayerEventTrigger>();
        else
            triggersInLevel.Clear();

        var searched = GetComponentsInChildren<PlayerEventTrigger>();
        foreach (var item in searched)
        {
            if (triggersInLevel.Contains(item))
                continue;
            triggersInLevel.Add(item);
        }
    }

    public void ReloadRoomRulesetDatas(List<Level4RoomRuleset> datas)
    {
        if (roomRulesets == null)
            roomRulesets = new List<Level4RoomRuleset>();
        else
            roomRulesets.Clear();

        roomRulesets = datas;
    }
#endif
}