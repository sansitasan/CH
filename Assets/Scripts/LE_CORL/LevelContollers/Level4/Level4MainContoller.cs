using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class Level4MainContoller : MonoBehaviour
{
    [Header("Object Refer")]
    [SerializeField] List<PlayerEventTrigger> trigger;
    [Space(10)]

    [Header("Timeline")]
    [SerializeField] PlayableDirector playerableDicetor;
    [SerializeField] TimelinePreferences[] timelinePreferences;
    [Space(10)]

    [Header("Datas")]
    [SerializeField] List<Level4RoomRuleset> roomRulesets;

    public event EventHandler<RoomStateChangedEventArgs> OnRoomStateChanged;
    public class RoomStateChangedEventArgs: EventArgs
    {
        public bool isStartRoom;
        public Level4RoomRuleset roomRulesetData;
    }

    private void Awake()
    {
        
    }

    private void OnEnable()
    {
        PlayerEventTrigger.OnPlayerEntered += PlayerEventTrigger_OnPlayerEntered;
        PlayerEventTrigger.OnPlayerExited += PlayerEventTrigger_OnPlayerExited;
        trigger[0].SetTriggerActivation(true);
    }


    private void PlayerEventTrigger_OnPlayerEntered(object sender, System.EventArgs e)
    {
        PlayerEventTrigger trigger = (PlayerEventTrigger)sender;
        var id = trigger.EventID;
        print($"player trigger enter: {id}");

        if (!id.Contains("level4_"))
            return;
        if (id.Contains("room"))
        {
            int num = int.Parse(id.Split("room")[1]);
            OnRoomStateChanged.Invoke(sender, new RoomStateChangedEventArgs
            {
                isStartRoom = true,
                roomRulesetData = roomRulesets[num]
            });
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
            if (trigger.Contains(item))
                continue;
            trigger.Add(item);
        }
    }
}