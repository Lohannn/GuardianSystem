using System;
using UnityEngine;
using UnityEngine.UI;

public class CallButtonView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button callButton;
    [SerializeField] private Text callButtonText; //Protype only
    public ObjectPool ownerPool;

    [Header("Event Data")]
    [SerializeField] private EventData data;
    [SerializeField] private float remainingTime;

    public void Initialize(EventData eventData)
    {
        data = eventData;
        remainingTime = data.eventDuration;

        callButtonText.text = data.eventType.ToString();

        callButton.onClick.RemoveAllListeners();
        callButton.onClick.AddListener(OnCallButtonClicked);
    }

    void Update()
    {
        if (GameManager.Instance.CurrentGameState == GameState.Running ||
            GameManager.Instance.CurrentGameState == GameState.Overtime)
        {
            if (remainingTime > 0)
            {
                remainingTime -= Time.deltaTime;
                if (remainingTime <= 0)
                {
                    remainingTime = 0;
                    OnTimeout();
                }
            }
        }
    }

    private void OnCallButtonClicked()
    {
        Debug.Log("Call button clicked");
        EventBus.OnCallClicked?.Invoke(data, GetComponent<CallButtonPresenter>());
    }

    private void OnTimeout()
    {
        Debug.Log("Call timed out");
        EventBus.OnCallTimedOut?.Invoke(data, GetComponent<CallButtonPresenter>());
    }
}
