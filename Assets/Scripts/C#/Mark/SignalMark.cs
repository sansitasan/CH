using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class SignalMark : Marker, INotification
{
    public EventTypes Event;
    public PropertyName id { get { return new PropertyName((int)Event); } }
}
