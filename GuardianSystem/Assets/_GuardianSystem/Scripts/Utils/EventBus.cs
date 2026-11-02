using System;
using UnityEngine;

public static class EventBus
{
    public static Action<EventData, CallButtonPresenter> OnCallTimedOut;
    public static Action<EventData, CallButtonPresenter> OnCallResolved;
    public static Action<EventData, CallButtonPresenter> OnCallClicked;
}
