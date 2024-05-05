using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class MarkBridge : MonoBehaviour, INotificationReceiver
{
    public GameObject GO;

    public void OnNotify(Playable origin, INotification notification, object context)
    {
         GameScene.Instance.GetEvent((EventTypes)notification.id.GetHashCode());
    }
}
