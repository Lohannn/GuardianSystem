using System;
using UnityEngine;

public class CallButtonPresenter : MonoBehaviour
{
    [SerializeField] private CallButtonView view;

    private Action callEndedAction;

    private GameManager gameManager;

    private void Awake()
    {
        gameManager = GameManager.Instance;
    }

    void OnEnable()
    {
        EventBus.OnCallTimedOut += CallTimeOutHandler;
        EventBus.OnCallResolved += CaseResolvedHandler;
    }

    void OnDisable()
    {
        EventBus.OnCallTimedOut -= CallTimeOutHandler;
        EventBus.OnCallResolved -= CaseResolvedHandler;
    }

    public void SetOnCallEnded(Action eventToInvoke)
    {
        callEndedAction = eventToInvoke;
    }

    public void CaseResolvedHandler(EventData eventData, CallButtonPresenter callPresenter)
    {
        if (callPresenter != this) return;

        if (callEndedAction != null) callEndedAction?.Invoke();

        gameManager.AdjustActiveCalls(-1);
        gameManager.TryEndDay();

        view.DisableAnimation();
    }

    private void CallTimeOutHandler(EventData eventData, CallButtonPresenter callPresenter)
    {   
        if (callPresenter != this) return;

        if (callEndedAction != null) callEndedAction?.Invoke();

        gameManager.populationModel.AdjustPopularity(eventData.popularityImpactOnTimeout);
        gameManager.AdjustActiveCalls(-1);
        gameManager.TryEndDay();

        view.DisableAnimation();
    }
}
