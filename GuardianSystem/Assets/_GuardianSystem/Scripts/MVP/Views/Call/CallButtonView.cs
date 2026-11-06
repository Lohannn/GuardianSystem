using System;
using UnityEngine;
using UnityEngine.UI;

public class CallButtonView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button callButton;
    [SerializeField] private Image timeViewerImage;
    public ObjectPool ownerPool;

    [Header("Event Data")]
    [SerializeField] private EventData data;
    [SerializeField] private float maxTime;
    [SerializeField] private float remainingTime;

    public void Initialize(EventData eventData)
    {
        data = eventData;
        maxTime = data.eventDuration;
        remainingTime = 0;

        timeViewerImage.fillAmount = 0;

        callButton.onClick.RemoveAllListeners();
        callButton.onClick.AddListener(OnCallButtonClicked);
    }

    void Update()
    {
        if (GameManager.Instance.CurrentGameState == GameState.Running ||
            GameManager.Instance.CurrentGameState == GameState.Overtime)
        {
            if (remainingTime < maxTime)
            {
                remainingTime += Time.deltaTime;
                timeViewerImage.fillAmount = Mathf.Clamp01(remainingTime / maxTime);

                if (remainingTime >= maxTime)
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
