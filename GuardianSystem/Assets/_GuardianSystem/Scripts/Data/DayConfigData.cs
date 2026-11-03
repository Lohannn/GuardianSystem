using UnityEngine;

[CreateAssetMenu(fileName = "New Day Configuration", menuName = "GS/Day Configuration")]
public class DayConfigData : ScriptableObject
{
    public int DayNumber;

    [Header("Rhythm and Difficulty")]
    public float dayDurationInSeconds = 180f;
    public float minCallSpawnInterval = 3f;
    public float maxCallSpawnInterval = 7f;
    public int maxSimultaneousCalls;
    public float waterVariationPerSecond;
    public float waterDirectionChangeInterval;

    public float popularityImpactOnWaterCallSpawn;
    public float emptyWaterInterval;
    public float popularityImpactOnEmptyWaterPerTick;

    [Header("Active Mechanics and Call Types")]
    public bool isTutorialActive = false;
    public EventWeight[] activeEventWeights;
}
