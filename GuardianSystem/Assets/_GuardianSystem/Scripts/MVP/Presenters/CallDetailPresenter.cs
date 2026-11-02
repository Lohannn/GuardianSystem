using UnityEngine;

public class CallDetailPresenter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CallDetailView view;

    [Header("Event Data")]
    private CallButtonPresenter callButtonPresenter;
    private EventAnswer? currentSelectedAnswer;

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
        callButtonPresenter = callPresenter;
    }

    public void CallResolvedHandler(EventData eventData)
    {
        if (currentSelectedAnswer == null)
        {
            Debug.LogWarning("No answer selected. Cannot resolve the call.");
            return;
        }

        EventBus.OnCallResolved?.Invoke(eventData, callButtonPresenter);
        gameManager.populationModel.AdjustPopularity(currentSelectedAnswer.Value.popularityImpact);
        view.Hide();
    }

    private void CallTimeOutHandler(EventData eventData, CallButtonPresenter callPresenter)
    {
        if (callButtonPresenter != callPresenter) return;
        view.Hide();
    }
}
