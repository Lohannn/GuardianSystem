using System;
using UnityEngine;
using UnityEngine.UI;

public class WaterLevelSlider : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Slider zoneLevelSlider;

    [Header("Zone config")]
    [SerializeField] private WaterZone sourceZone;

    public WaterZone SourceZone => sourceZone;

    public void AddOnValueChanged(Action<WaterZone, float> eventToTrigger)
    {
        zoneLevelSlider.onValueChanged.RemoveAllListeners();
        zoneLevelSlider.onValueChanged.AddListener(
            value => eventToTrigger?.Invoke(sourceZone, value)
        );
    }

    public void SetValue(float value)
    {
        zoneLevelSlider.SetValueWithoutNotify(value);
    }
}
