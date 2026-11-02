using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitItemPresenter : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [Header("References")]
    [SerializeField] private UnitItemView view;

    [Header("Unit Info")]
    [SerializeField] private UnitSO unit;
    [SerializeField] private RectTransform unitBase;

    public UnitSO Unit => unit;
    public RectTransform UnitBase => unitBase;

    private Action dispatchResult;

    private bool inDispatch = false;

    void OnEnable()
    {
        EventBus.OnDispatchStart += OnDispatchStartHandler;
    }

    void OnDisable()
    {
        EventBus.OnDispatchStart -= OnDispatchStartHandler;
    }

    void Start()
    {
        if (view == null)
            view = GetComponent<UnitItemView>();
    }

    private void OnDispatchStartHandler(UnitItemPresenter presenter, float dispatchedTime, Action result)
    {
        if (presenter != this) return;

        dispatchResult = result;
        inDispatch = true;
        view.StartDispatch(dispatchedTime);
    }

    public void EndDispatchMode()
    {
        dispatchResult?.Invoke();
        dispatchResult = null;
        inDispatch = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (inDispatch)
        {
            eventData.pointerDrag = null;
            return;
        }

        EventBus.OnUnitBeginDrag?.Invoke(this, eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (inDispatch) return;

        EventBus.OnUnitDrag?.Invoke(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (inDispatch) return;

        EventBus.OnUnitEndDrag?.Invoke();
    }
}
