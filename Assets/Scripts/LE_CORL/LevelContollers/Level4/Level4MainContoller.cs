using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using Assets.Scripts.LE_CORL.Player;

public class Level4MainContoller : MonoBehaviour
{
    public const string FALLING_OBJECT_ID = "falling_object_";

    public static Level4MainContoller Instance;

    [Header("Object Refer")]
    [SerializeField] List<PlayerEventTrigger> timelineTriggers;

    [SerializeField] PlayerControllerBase playerController;

    [Header("Timeline")]
    [SerializeField] PlayableDirector playerableDicetor;

    [Header("Prefabs")]
    [SerializeField] GameObject[] fallingObjectPrefabs;

    public Transform roomTriggersParent;

    public static event EventHandler OnGameOver;


    private void Awake()
    {
        Instance = this;
    }


    private void OnEnable()
    {
        PlayerEventTrigger.OnPlayerEntered += PlayerEventTrigger_OnPlayerEntered;
        PlayerEventTrigger.OnPlayerExited += PlayerEventTrigger_OnPlayerExited;

        // 메모리풀 등록
        for (int i = 0; i < fallingObjectPrefabs.Length; i++)
        {
            var go = fallingObjectPrefabs[i];
            var id = FALLING_OBJECT_ID + i;
            MemoryPoolManager.RegisterMemorypoolObj(id, go);
        }
    }

    private void OnDestroy()
    {
        PlayerEventTrigger.OnPlayerEntered -= PlayerEventTrigger_OnPlayerEntered;
        PlayerEventTrigger.OnPlayerExited -= PlayerEventTrigger_OnPlayerExited;

        Instance = null;
    }

    private void PlayerEventTrigger_OnPlayerEntered(object sender, System.EventArgs e)
    {
        PlayerEventTrigger trigger = (PlayerEventTrigger)sender;
        var id = trigger.EventID;
        print($"player trigger enter: {id}");

        if (!id.StartsWith("level4_"))
            return;

        else if (id.Contains("timeline_"))
        {

        }
        else if (id.Contains("fallingObject_"))
        {

        }
    }

    private void PlayerEventTrigger_OnPlayerExited(object sender, EventArgs e)
    {
        return;
        /*
        PlayerEventTrigger trigger = (PlayerEventTrigger)sender;
        var id = trigger.EventID;
        print($"player trigger exit: {id}");
        */
    }

    public void GameOver(PlayerControllerBase player)
    {
        print("game over");
    }

    public void PlayTimeline(TimelinePreferences timeline)
    {
        playerableDicetor.Play();
    }
}