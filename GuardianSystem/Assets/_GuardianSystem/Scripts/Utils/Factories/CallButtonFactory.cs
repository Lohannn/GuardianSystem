using System.Collections.Generic;
using UnityEngine;

public class CallButtonFactory : MonoBehaviour
{
    [Header("Factory Configuration")]
    [SerializeField] private EventDatabase eventDatabase;
    [SerializeField] private RectTransform[] spawnPoints;

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
        gameManager.SpawnCall += SpawnCallButton;
        gameManager.OnDayStarted += ChangeAvailableEventsForTheDay;
    }

    private void OnDisable()
    {
        gameManager.SpawnCall -= SpawnCallButton;
        gameManager.OnDayStarted -= ChangeAvailableEventsForTheDay;
    }

    private void Start()
    {        
        availableEventsForTheDay = eventDatabase.GetEventsByType(gameManager.currentDayConfig.activeEventWeights);
    }

    private void ChangeAvailableEventsForTheDay()
    {
        availableEventsForTheDay = eventDatabase.GetEventsByType(gameManager.currentDayConfig.activeEventWeights);
    }

    private void SpawnCallButton()
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

        buttonObj.SetActive(true);

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
