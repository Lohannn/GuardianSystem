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

    //Events
    public event Action<EventData> CallResolved;
    public event Action<EventData> CallTimedOut;

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

    private void OnCallButtonClicked()
    {
        Debug.Log("Call resolved");
        CallResolved?.Invoke(data);
    }

    private void OnTimeout()
    {
        Debug.Log("Call timed out");
        CallTimedOut?.Invoke(data);
    }
}
