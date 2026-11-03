using System;
using System.Collections.Generic;
using UnityEngine;

public enum WaterZone
{
    South,
    North,
    Center,
    West,
    East
}

public class WaterLevelModel
{
    private Dictionary<WaterZone, float> zoneLevels = new();
    private Dictionary<WaterZone, int> zoneDirections = new(); 
    private float variationPerSecond;
    private float directionChangeInterval;
    private float directionTimer;

    public WaterLevelModel(float variation, float changeInterval)
    {
        variationPerSecond = variation;
        directionChangeInterval = changeInterval;

        foreach (WaterZone zone in Enum.GetValues(typeof(WaterZone)))
        {
            zoneLevels[zone] = 50f;
        }
    }

    public void UpdateValues(float deltaTime)
    {
        directionTimer += deltaTime;

        if (directionTimer >= directionChangeInterval)
        {
            ChangeDirections();
            directionTimer = 0f;
        }

        foreach (WaterZone zone in zoneLevels.Keys)
        {
            float currentLevel = zoneLevels[zone];

            if (currentLevel <= 0 || currentLevel >= 100) continue;

            zoneLevels[zone] += zoneDirections[zone] * variationPerSecond * deltaTime;

            zoneLevels[zone] = Mathf.Clamp(zoneLevels[zone], 0, 100);
            
            if (zoneLevels[zone] >= 100)
            {
                EventBus.OnWaterOverflow?.Invoke(zone);
            }
        }
    }

    private void ChangeDirections()
    {
        foreach (WaterZone zone in zoneDirections.Keys)
        {
            zoneDirections[zone] = (int)(UnityEngine.Random.value < 0.5f ? -1 : 1f);
        }
    }

    public float GetZoneLevel(WaterZone zone)
    {        
        return zoneLevels[zone];
    }

    public void SetZoneLevel(WaterZone zone, float value)
    {   
        zoneLevels[zone] = Mathf.Clamp(value, 0, 100);

        if (zoneLevels[zone] >= 100)
        {
            EventBus.OnWaterOverflow?.Invoke(zone);
        }
    }
}
