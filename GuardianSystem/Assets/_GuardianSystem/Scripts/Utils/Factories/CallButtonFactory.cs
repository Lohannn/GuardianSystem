using System.Collections.Generic;
using UnityEngine;

public class CallButtonFactory : MonoBehaviour
{
    [Header("Factory Configuration")]
    [SerializeField] private EventDatabase eventDatabase;
    [SerializeField] private RectTransform[] spawnPoints;

    [Header("Water Call creation configuration")]
    [SerializeField] private List<ZonePresenter> waterSpawnPoints;
    [SerializeField] private EventDatabase waterEventDatabase;

    private List<EventData> availableEventsForTheDay = new List<EventData>();

    private ObjectPool buttonPool;
    private GameManager gameManager;

    void Awake()
    {
        buttonPool = GetComponent<ObjectPool>();
        gameManager = GameManager.Instance;
    }

    private void OnEnable()
    {
        gameManager.SpawnCall += SetupCallButton;
        gameManager.OnDayStarted += ChangeAvailableEventsForTheDay;

        EventBus.OnWaterOverflow += SetupWaterCallButton;
    }

    private void OnDisable()
    {
        gameManager.SpawnCall -= SetupCallButton;
        gameManager.OnDayStarted -= ChangeAvailableEventsForTheDay;
        
        EventBus.OnWaterOverflow -= SetupWaterCallButton;
    }

    private void Start()
    {        
        availableEventsForTheDay = eventDatabase.GetEventsByType(gameManager.currentDayConfig.activeEventWeights);
    }

    private void ChangeAvailableEventsForTheDay()
    {
        availableEventsForTheDay = eventDatabase.GetEventsByType(gameManager.currentDayConfig.activeEventWeights);
    }

    private void SetupCallButton()
    {
        if (availableEventsForTheDay.Count == 0)
        {
            Debug.LogWarning("No available events for the day.");
            return;
        }

        EventData randomEvent = availableEventsForTheDay[Random.Range(0, availableEventsForTheDay.Count)];
        
        RectTransform spawnPoint = GetRandomSpawnPoint();

        GameObject buttonObj = buttonPool.GetObjectFromPool();
        RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();

        buttonRect.SetParent(spawnPoint, false);
        buttonRect.localPosition = Vector3.zero;

        CallButtonView callButtonView = buttonObj.GetComponent<CallButtonView>();
        callButtonView.Initialize(randomEvent);
        callButtonView.ownerPool = buttonPool;

        SpawnCallButton(buttonObj);
    }

    private void SetupWaterCallButton(Zone zone)
    {
        ZonePresenter affectedZone = waterSpawnPoints.Find(sourceZone => sourceZone.Zone == zone);

        if (affectedZone == null)
        {
            Debug.LogWarning($"No water spawn point found for zone {zone}.");
            return;
        }

        if (affectedZone.currentCall != null) return;

        if (waterEventDatabase.Events.Count == 0)
        {
            Debug.LogWarning("No available events for water calls.");
            return;
        }

        EventData randomEvent = waterEventDatabase.Events[Random.Range(0, waterEventDatabase.Events.Count)];
        
        RectTransform spawnPoint = affectedZone.GetComponent<RectTransform>();

        GameObject buttonObj = buttonPool.GetObjectFromPool();
        RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();

        buttonRect.SetParent(spawnPoint, false);
        buttonRect.localPosition = Vector3.zero;

        CallButtonView callButtonView = buttonObj.GetComponent<CallButtonView>();
        callButtonView.Initialize(randomEvent);
        callButtonView.ownerPool = buttonPool;

        affectedZone.currentCall = buttonObj.GetComponent<CallButtonPresenter>();

        SpawnCallButton(buttonObj);
        gameManager.populationModel.AdjustPopularity(gameManager.currentDayConfig.popularityImpactOnWaterCallSpawn);
    }

    private void SpawnCallButton(GameObject call)
    {
        call.SetActive(true);
        gameManager.AdjustActiveCalls(1);
    }

    private RectTransform GetRandomSpawnPoint()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points defined for CallButtonFactory.");
            return null;
        }

        RectTransform selectedSpawn = spawnPoints[Random.Range(0, spawnPoints.Length)];

        while (selectedSpawn.childCount > 0)
        {
            selectedSpawn = spawnPoints[Random.Range(0, spawnPoints.Length)];
        }
        
        return selectedSpawn;
    }
}
