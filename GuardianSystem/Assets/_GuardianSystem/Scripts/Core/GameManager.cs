using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Setup,
    Starting,
    Running,
    Paused,
    EndOfDay,
    Cutscene
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State manager")]
    public GameState CurrentGameState { get; private set; } = GameState.Starting;
    private int currentCallsActive = 0;

    [Header("Day configuration")]
    [SerializeField] private List<DayConfigData> dayConfigOrder;
    public DayConfigData currentDayConfig;
    [NonSerialized] public PopulationModel populationModel;

    [Header("Day Data")]
    private int currentDayNumber = 0;
    private float currentDayDuration;

    //Events
#region Game State Events
    public event Action OnDayStarted;
    public event Action OnDayPaused;
    public event Action OnDayRunning;
    public event Action OnDayEnded;
    public event Action OnCutsceneStarted;
#endregion

    public event Action SpawnCall;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ChangeState(GameState.Starting);
    }

    private void Update()
    {
        if (CurrentGameState == GameState.Running)
        {
            currentDayDuration -= Time.deltaTime;

            if (currentDayDuration <= 0)
            {
                ChangeState(GameState.EndOfDay);
            }
        }
    }

    public void ChangeState(GameState newState)
    {
        CurrentGameState = newState;

        switch (newState)
        {
            case GameState.Setup:
                Debug.Log("Game is setting up.");
                SetupDay();
                break;
            case GameState.Starting:
                Debug.Log("Game is starting.");
                populationModel = new PopulationModel();
                StartDay();
                OnDayStarted?.Invoke();
                break;
            case GameState.Running:
                Debug.Log("Game is now running.");
                StartCoroutine(SpawnCallCoroutine());
                OnDayRunning?.Invoke();
                break;
            case GameState.Paused:
                Debug.Log("Game is now paused.");
                OnDayPaused?.Invoke();
                break;
            case GameState.EndOfDay:
                Debug.Log("Day has ended.");
                OnDayEnded?.Invoke();
                break;
            case GameState.Cutscene:
                Debug.Log("Cutscene is now playing.");
                OnCutsceneStarted?.Invoke();
                break;
        }
    }

    private void SetupDay()
    {
        if (currentDayNumber < dayConfigOrder.Count)
        {
            if (currentDayNumber == 3)
            {
                if (populationModel.Popularity < 30)
                {
                    Debug.Log("Popularity is below 30, choosing bad end day.");
                    // Set the current day config to the bad end day
                }
                else
                {
                    Debug.Log("Popularity is above 30, choosing good end day.");
                    // Set the current day config to the good end day
                }
            }
            else
            {
                currentDayConfig = dayConfigOrder[currentDayNumber];
            }
        }
        else
        {
            currentDayConfig = dayConfigOrder[dayConfigOrder.Count - 1];
        }

        Debug.Log($"Setting up Day {currentDayConfig.name}.");
        ChangeState(GameState.Starting);
    }

    private void StartDay()
    {
        if (currentDayConfig != null)
        {
            currentDayNumber = currentDayConfig.DayNumber;
            currentDayDuration = currentDayConfig.dayDurationInSeconds;
            StartCoroutine(StartDayCoroutine());
        }
        else
        {
            Debug.LogError("No DayConfigData assigned to GameManager.");
        }
    }

    public void AdjustActiveCalls(int adjustment)
    {
        currentCallsActive += adjustment;
    }

    private IEnumerator StartDayCoroutine()
    {
        yield return new WaitForSeconds(1.5f);
        ChangeState(GameState.Running);
    }

    private IEnumerator SpawnCallCoroutine()
    {
        while (CurrentGameState == GameState.Running)
        {
            if (currentCallsActive >= currentDayConfig.maxSimultaneousCalls)
            {
                yield return new WaitForSeconds(0.5f);
                continue;
            }

            float spawnInterval = UnityEngine.Random.Range(currentDayConfig.minCallSpawnInterval, currentDayConfig.maxCallSpawnInterval);

            yield return new WaitForSeconds(spawnInterval);

            if (CurrentGameState == GameState.Running && currentCallsActive < currentDayConfig.maxSimultaneousCalls)
            {
                SpawnCall?.Invoke();
            }
        }
    }
}
