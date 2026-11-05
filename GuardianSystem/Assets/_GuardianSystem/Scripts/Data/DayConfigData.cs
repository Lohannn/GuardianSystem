using UnityEngine;

[CreateAssetMenu(fileName = "New Day Configuration", menuName = "GS/Day Configuration")]
public class DayConfigData : ScriptableObject
{
    public int DayNumber;
    public CutsceneData endDayCutscene;

    [Header("Calls")]
    public float dayDurationInSeconds = 180f;
    public float minCallSpawnInterval = 3f;
    public float maxCallSpawnInterval = 7f;
    public int maxSimultaneousCalls;

    [Header("Water")]
    public float waterVariationPerSecond;
    public float waterDirectionChangeInterval;
    public float popularityImpactOnWaterCallSpawn;
    public float emptyWaterTickInterval;
    public float popularityImpactOnEmptyWaterPerTick;

    [Header("Trash")]
    public float minTrashAddInterval = 4f;
    public float maxTrashAddInterval = 10f;
    public float fullTrashTickInterval;
    public float popularityImpactOnFullTrashPerTick;
    public float popularityImpactOnCollectedTrash;

    [Header("Active Mechanics and Call Types")]
    public bool isTutorialActive = false;
    public EventWeight[] activeEventWeights;
}
