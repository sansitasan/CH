using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class PlayerEventTrigger : MonoBehaviour
{
    [SerializeField] Collider2D collider2d;
    [SerializeField] string eventID;

    /// <summary>
    /// ID 명명 규칙:
    ///     모두 영문 소문자 사용.
    ///     {사용되는 씬의 이름}_{오브젝트 이름}_{관리 번호}
    /// </summary>
    public string EventID { get { return eventID; } }

    public static event EventHandler OnPlayerEntered;
    public static event EventHandler OnPlayerExited;


    public void Init(string eventID)
    {
        collider2d = GetComponent<Collider2D>();
        collider2d.enabled = false;
        this.eventID = eventID;
    }

    public void SetTriggerActivation(bool isOn)
    {
        collider2d.enabled = isOn;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        OnPlayerEntered?.Invoke(this, EventArgs.Empty);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        OnPlayerExited?.Invoke(this, EventArgs.Empty);
    }
}
