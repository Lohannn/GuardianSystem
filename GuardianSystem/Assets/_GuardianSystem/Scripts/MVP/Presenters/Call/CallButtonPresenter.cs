using System;
using UnityEngine;

public class CallButtonPresenter : MonoBehaviour
{
    [SerializeField] private CallButtonView view;

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

    //Por agora, apenas pra chamados de pergunta
    public void CaseResolvedHandler(EventData eventData, CallButtonPresenter callPresenter)
    {
        if (callPresenter != this) return;

        gameManager.AdjustActiveCalls(-1);
        gameManager.TryEndDay();

        view.DisableAnimation();
    }

    private void CallTimeOutHandler(EventData eventData, CallButtonPresenter callPresenter)
    {   
        if (callPresenter != this) return;

        gameManager.populationModel.AdjustPopularity(eventData.popularityImpactOnTimeout);
        gameManager.AdjustActiveCalls(-1);
        gameManager.TryEndDay();

        view.DisableAnimation();
    }
}
