using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Scripting;

public class CallDispatchSlotPresenter : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    [SerializeField] private CallDispatchSlotView view;
    [SerializeField] private int slotPosition;

    public CallDetailPresenter callDetail;

    [Header("Unit Info")]
    private UnitItemPresenter currentUnitInSlot;

    void OnEnable()
    {
        EventBus.OnCallResolved += DispatchUnits;
        EventBus.OnCallTimedOut += ClearSlot;
    }

    void OnDisable()
    {
        EventBus.OnCallResolved -= DispatchUnits;       
        EventBus.OnCallTimedOut -= ClearSlot;
    }

    public void OnDrop(PointerEventData eventData)
    {
        currentUnitInSlot = eventData.pointerDrag?.GetComponent<UnitItemPresenter>();

        if (currentUnitInSlot == null) return;

        bool accepted = callDetail.TryUnitDrop(slotPosition, currentUnitInSlot);
        
        if (accepted) view.SetIcon(currentUnitInSlot.Unit);
    }

    private void ClearSlot(EventData data, CallButtonPresenter call)
    {
        currentUnitInSlot = null;
        view.RemoveIcon();
        EventBus.OnHideDispatchPreview?.Invoke();
    }

    private void DispatchUnits(EventData eventData, CallButtonPresenter call)
    {
        if (eventData.eventType == EventType.Dispatch && currentUnitInSlot != null)
        {
            Vector2 callPosition = call.GetComponent<RectTransform>().position;
            EventBus.OnDispatchStart?.Invoke(
                currentUnitInSlot, 
                TimeBetweenUnitAndCall(currentUnitInSlot, callPosition), 
                callDetail.result
            );
        }
    }

    private float TimeBetweenUnitAndCall(UnitItemPresenter unit, Vector2 callPosition)
    {
        float distance = Vector2.Distance(unit.UnitBase.position, callPosition);

        return distance / unit.Unit.speedBySecond;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!eventData.pointerDrag) return;

        UnitItemPresenter unitPreview = eventData.pointerDrag?.GetComponent<UnitItemPresenter>();;

        EventBus.OnShowDispatchPreview?.Invoke(TimeBetweenUnitAndCall(unitPreview, callDetail.currentCallButtonPresenter.transform.position));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!eventData.pointerDrag) return;

        EventBus.OnHideDispatchPreview?.Invoke();
    }
}
