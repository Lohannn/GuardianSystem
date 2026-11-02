using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CallDetailPresenter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CallDetailView view;

    [Header("Event Data")]
    public CallButtonPresenter currentCallButtonPresenter;
    private EventAnswer? currentSelectedAnswer;
    private UnitType?[] dispatchedUnits = new UnitType?[2];
    public Action result;

    private GameManager gameManager;

    void Awake()
    {
        gameManager = GameManager.Instance;
    }

    void OnEnable()
    {
        EventBus.OnCallClicked += HandleCallClicked;
        EventBus.OnCallTimedOut += CallTimeOutHandler;
    }

    void OnDisable()
    {
        EventBus.OnCallClicked -= HandleCallClicked;
        EventBus.OnCallTimedOut -= CallTimeOutHandler;
    }

    public void HandleAnswerSelection(EventAnswer selectedAnswer)
    {
        currentSelectedAnswer = selectedAnswer;
    }

    private void HandleCallClicked(EventData eventData, CallButtonPresenter callPresenter)
    {
        currentCallButtonPresenter = callPresenter;
    }

    public bool TryUnitDrop(int dispatchSlot, UnitItemPresenter unitItem)
    {
        UnitType typeReceived = unitItem.Unit.type;

        if (dispatchedUnits.Contains(typeReceived)) return false;    

        dispatchedUnits[dispatchSlot] = typeReceived;
        return true;
    }

    public void CallResolvedHandler(EventData eventData)
    {
        if (currentCallButtonPresenter == null) return;

        if (eventData.eventType == EventType.Question)
        {
            if (currentSelectedAnswer == null)
            {
                Debug.LogWarning("No answer selected. Cannot resolve the call.");
                return;
            }

            gameManager.populationModel.AdjustPopularity(currentSelectedAnswer.Value.popularityImpact);
        } 
        else
        {
            int emptySlots = dispatchedUnits.Count(slot => slot == null);

            if ((dispatchedUnits.Length - emptySlots) < eventData.requiredUnits.Length)
            {
                Debug.LogWarning("Units remaining to Dispatch. Cannot resolve the call.");
                return;
            }

            if (!eventData.requiredUnits.All(unit => dispatchedUnits.Contains(unit)))
            {
                result = () => gameManager.populationModel.AdjustPopularity(eventData.popularityImpactOnWrongUnits);
            }else
            {
                result = () => gameManager.populationModel.AdjustPopularity(eventData.popularityImpactOnSuccess);
            }
        }

        EventBus.OnCallResolved?.Invoke(eventData, currentCallButtonPresenter);
        
        currentSelectedAnswer = null;
        for (int i = 0; i < dispatchedUnits.Length; i++)
        {
            dispatchedUnits[i] = null;
        }
        result = null;

        view.Hide();
    }

    private void CallTimeOutHandler(EventData eventData, CallButtonPresenter callPresenter)
    {
        if (currentCallButtonPresenter != callPresenter) return;
        view.Hide();
    }
}
