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
    private float penaltyInterval;
    private float penaltyValue;

    private float directionTimer;
    private float penaltyTimer;

    private GameManager gameManager;

    public WaterLevelModel(GameManager gm)
    {
        DayConfigData dayConfig = gm.currentDayConfig;

        gameManager = gm;
        variationPerSecond = dayConfig.waterVariationPerSecond;
        directionChangeInterval = dayConfig.waterDirectionChangeInterval;
        penaltyInterval = dayConfig.emptyWaterInterval;
        penaltyValue = dayConfig.popularityImpactOnEmptyWaterPerTick;

        foreach (WaterZone zone in Enum.GetValues(typeof(WaterZone)))
        {
            zoneLevels.Add(zone, 50f);
            zoneDirections.Add(zone, UnityEngine.Random.value > 0.5f ? -1 : 1);
        }
    }

    public Dictionary<WaterZone, float> UpdateValues(float deltaTime)
    {
        directionTimer += deltaTime;
        penaltyTimer += deltaTime;

        if (directionTimer >= directionChangeInterval)
        {
            ChangeDirections();
            directionTimer = 0f;
        }

        if (penaltyTimer >= penaltyInterval)
        {
            penaltyTimer = 0f;
            int emptyZones = 0;

            foreach (float zone in zoneLevels.Values)
                if (zone <= 0) emptyZones++;

            if (emptyZones > 0)
            {
                float totalPenalty = emptyZones * penaltyValue;
                gameManager.populationModel.AdjustPopularity(totalPenalty);
            }
        }

        foreach (WaterZone zone in new List<WaterZone>(zoneLevels.Keys))
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
        return zoneLevels;
    }

    private void ChangeDirections()
    {
        foreach (WaterZone zone in Enum.GetValues(typeof(WaterZone)))
        {
            zoneDirections[zone] = UnityEngine.Random.value < 0.5f ? -1 : 1;
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
