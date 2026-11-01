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
        view.CallResolved += CaseResolvedHandler;
        view.CallTimedOut += CallTimeOutHandler;
    }

    void OnDisable()
    {
        view.CallResolved -= CaseResolvedHandler;
        view.CallTimedOut -= CallTimeOutHandler;
    }

    private void CaseResolvedHandler(EventData eventData)
    {
        gameManager.populationModel.AdjustPopularity(5);
        gameManager.AdjustActiveCalls(-1);
        view.ownerPool.ReturnObjectToPool(view.gameObject);
    }

    private void CallTimeOutHandler(EventData eventData)
    {   
        gameManager.populationModel.AdjustPopularity(eventData.popularityImpactOnTimeout);
        gameManager.AdjustActiveCalls(-1);
        view.ownerPool.ReturnObjectToPool(view.gameObject);
    }
}
