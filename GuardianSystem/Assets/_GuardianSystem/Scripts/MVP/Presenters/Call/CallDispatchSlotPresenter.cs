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
        EventBus.OnDetailPanelHide += ClearSlot;
    }

    void OnDisable()
    {
        EventBus.OnCallResolved -= DispatchUnits;       
        EventBus.OnCallTimedOut -= ClearSlot;
        EventBus.OnDetailPanelHide -= ClearSlot;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (GameManager.Instance.CurrentGameState != GameState.Running &&
            GameManager.Instance.CurrentGameState != GameState.Overtime) return;

        currentUnitInSlot = eventData.pointerDrag?.GetComponent<UnitItemPresenter>();

        if (currentUnitInSlot == null) return;

        bool accepted = callDetail.TryUnitDrop(slotPosition, currentUnitInSlot);
        
        if (accepted) view.SetIcon(currentUnitInSlot.Unit);
    }

    private void ClearSlot()
    {
        currentUnitInSlot = null;
        view.RemoveIcon();
        EventBus.OnHideDispatchPreview?.Invoke();
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
                UnitMovementCalculator.CalculateTravelTime(currentUnitInSlot.UnitBase.position, 
                                                           callPosition, 
                                                           currentUnitInSlot.Unit.speedBySecond), 
                slotPosition == 0 ? callDetail.result : null
            );
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!eventData.pointerDrag || 
            GameManager.Instance.CurrentGameState != GameState.Running &&
            GameManager.Instance.CurrentGameState != GameState.Overtime) return;

        UnitItemPresenter unitPreview = eventData.pointerDrag.GetComponent<UnitItemPresenter>();;

        if (unitPreview == null) return;

        EventBus.OnShowDispatchPreview?.Invoke(UnitMovementCalculator.CalculateTravelTime(
                                               unitPreview.UnitBase.position, 
                                               callDetail.currentCallButtonPresenter.transform.position,
                                               unitPreview.Unit.speedBySecond));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!eventData.pointerDrag) return;

        EventBus.OnHideDispatchPreview?.Invoke();
    }
}
