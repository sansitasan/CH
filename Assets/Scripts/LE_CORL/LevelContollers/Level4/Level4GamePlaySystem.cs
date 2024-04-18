using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level4GamePlaySystem : MonoBehaviour
{
    [SerializeField, Range(1, 5)] int healthMax = 3;
    [SerializeField] Transform lifeUIBundle;
    int health;

    public static event EventHandler OnPlayerHit;


    private void OnEnable()
    {
        PlayerEventTrigger.OnPlayerEntered += PlayerEventTrigger_OnPlayerEntered;
    }

    private void OnDisable()
    {
        PlayerEventTrigger.OnPlayerEntered -= PlayerEventTrigger_OnPlayerEntered;
    }

    private void PlayerEventTrigger_OnPlayerEntered(object sender, EventArgs e)
    {
        var tmp = (PlayerEventTrigger)sender;
        if (tmp == null) return;

        var id = tmp.EventID;
        if (!id.StartsWith("level4")) return;

        if (id.Contains("room_"))
        {
            health = healthMax;
            for (int i = 0; i < health; i++)
            {
                lifeUIBundle.GetChild(i).gameObject.SetActive(true);
            }
        }

        else if (id.Contains("fallingObject_"))
        {
            health--;
            lifeUIBundle.GetChild(0).gameObject.SetActive(false);
            if(health <= 0)
            {

            }
        }
    }




}
