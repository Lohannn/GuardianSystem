using System;
using System.Collections.Generic;
using UnityEngine;

public enum Zone
{
    South,
    North,
    Center,
    West,
    East
}

public class WaterLevelModel
{
    private Dictionary<Zone, float> zoneLevels = new();
    private Dictionary<Zone, int> zoneDirections = new(); 
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
        penaltyInterval = dayConfig.emptyWaterTickInterval;
        penaltyValue = dayConfig.popularityImpactOnEmptyWaterPerTick;

        foreach (Zone zone in Enum.GetValues(typeof(Zone)))
        {
            zoneLevels.Add(zone, 50f);
            zoneDirections.Add(zone, UnityEngine.Random.value > 0.5f ? -1 : 1);
        }
    }

    public Dictionary<Zone, float> UpdateValues(float deltaTime)
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

        foreach (Zone zone in new List<Zone>(zoneLevels.Keys))
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
        foreach (Zone zone in Enum.GetValues(typeof(Zone)))
        {
            zoneDirections[zone] = UnityEngine.Random.value < 0.5f ? -1 : 1;
        }
    }

    public float GetZoneLevel(Zone zone)
    {        
        return zoneLevels[zone];
    }

    public void SetZoneLevel(Zone zone, float value)
    {   
        zoneLevels[zone] = Mathf.Clamp(value, 0, 100);

        if (zoneLevels[zone] >= 100)
        {
            EventBus.OnWaterOverflow?.Invoke(zone);
        }
    }
}
