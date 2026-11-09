using System;
using UnityEngine;
using UnityEngine.UI;

public class WaterLevelSlider : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Slider zoneLevelSlider;
    [SerializeField] private Image fillArea;

    [Header("Zone config")]
    [SerializeField] private Zone sourceZone;

    public Zone SourceZone => sourceZone;

    public void AddOnValueChanged(Action<Zone, float> eventToTrigger)
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
