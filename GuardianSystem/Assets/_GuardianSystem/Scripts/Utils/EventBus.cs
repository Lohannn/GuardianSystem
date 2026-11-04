using System;
using UnityEngine;
using UnityEngine.EventSystems;

public static class EventBus
{
    public static Action<EventData, CallButtonPresenter> OnCallTimedOut;
    public static Action<EventData, CallButtonPresenter> OnCallResolved;
    public static Action<EventData, CallButtonPresenter> OnCallClicked;

    public static Action<UnitItemPresenter, Vector2> OnUnitBeginDrag;
    public static Action<Vector2> OnUnitDrag;
    public static Action OnUnitEndDrag;
    public static Action<UnitItemPresenter, float, Action> OnDispatchStart;
    public static Action<float> OnShowDispatchPreview;
    public static Action OnHideDispatchPreview;

    public static Action<Zone> OnWaterOverflow;
}
