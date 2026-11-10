using System;
using UnityEngine;
using UnityEngine.UI;

public class WaterLevelSlider : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Slider zoneLevelSlider;
    [SerializeField] private Image fillArea;

    [SerializeField] private Color32 crisisColor;
    private Color32 defaultColor;

    [Header("Zone config")]
    [SerializeField] private Zone sourceZone;
    public bool inCrisisState;

    public Zone SourceZone => sourceZone;

    private void Awake()
    {
        defaultColor = fillArea.color;
    }

    public void AddOnValueChanged(Action<Zone, float> eventToTrigger)
    {
        zoneLevelSlider.onValueChanged.RemoveAllListeners();
        zoneLevelSlider.onValueChanged.AddListener((value) =>
            {
                eventToTrigger?.Invoke(sourceZone, value);
                ToggleCrisisState(value <= 20 || value >= 80);
            }
        );
    }

    public void SetValue(float value)
    {
        zoneLevelSlider.SetValueWithoutNotify(value);
        ToggleCrisisState(value <= 20 || value >= 80);
    }

    public float GetValue()
    {
        return zoneLevelSlider.value;
    }

    private void ToggleCrisisState(bool inCrisis)
    {
        if (inCrisis)
        {
            fillArea.color = crisisColor;
            inCrisisState = true;
        }
        else
        {
            fillArea.color = defaultColor;
            inCrisisState = false;
        }
    }
}
