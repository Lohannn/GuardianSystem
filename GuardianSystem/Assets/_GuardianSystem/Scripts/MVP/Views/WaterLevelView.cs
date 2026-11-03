using System;
using System.Collections.Generic;
using UnityEngine;

public class WaterLevelView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<WaterLevelSlider> sliders;
    [SerializeField] private WaterLevelPresenter presenter;

    void Awake()
    {
        foreach (WaterLevelSlider slider in sliders)
        {
            slider.AddOnValueChanged(SetZoneLevelValue);
        }
    }

    private void SetZoneLevelValue(WaterZone zone, float newValue)
    {
        presenter.ChangeZoneLevel(zone, newValue);
    }

    public void UpdateSliders(Dictionary<WaterZone, float> zones)
    {
        foreach (WaterZone zone in zones.Keys)
        {
            WaterLevelSlider currentSlider = sliders.Find(slider => slider.SourceZone == zone);

            if (currentSlider == null)
            {
                Debug.LogWarning($"Cannot encounter a slider with the {zone} zone.");
                continue;
            }

            currentSlider.SetValue(zones[zone]);
        }
    }
}
