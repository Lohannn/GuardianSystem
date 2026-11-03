using UnityEngine;

public class WaterPosition : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WaterZone zone;

    public WaterZone Zone => zone;
    public CallButtonPresenter currentCall;

    void OnEnable()
    {
        EventBus.OnCallResolved += FreeWaterCallPosition;
        EventBus.OnCallTimedOut += FreeWaterCallPosition;
    }

    void OnDisable()
    {
        EventBus.OnCallResolved -= FreeWaterCallPosition;
        EventBus.OnCallTimedOut -= FreeWaterCallPosition;
    }

    private void FreeWaterCallPosition(EventData data, CallButtonPresenter call)
    {
        if (call != currentCall) return;

        currentCall = null;
    }
}
