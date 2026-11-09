using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Setup,
    Starting,
    Running,
    Overtime,
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
    private bool endOfDayRequested = false;

    //Events
#region Game State Events
    public event Action OnSetupDay;
    public event Action OnDayStarted;
    public event Action OnDayPaused;
    public event Action OnDayRunning;
    public event Action OnDayEnded;
    public event Action OnCutsceneStarted;
#endregion

    public event Action SpawnCall;
    public event Action<Zone> AddTrash;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        populationModel = new PopulationModel();
        ChangeState(GameState.Starting);
    }

    private void Update()
    {
        if (CurrentGameState == GameState.Running)
        {
            currentDayDuration -= Time.deltaTime;

            if (currentDayDuration <= 0)
            {
                ChangeState(GameState.Overtime);
                TryEndDay();
            }
        }
    }

    private void LateUpdate()
    {
        if (!endOfDayRequested) return;

        endOfDayRequested = false;

        if (CurrentGameState == GameState.Overtime && currentCallsActive == 0)
        {
            ChangeState(GameState.EndOfDay);
            currentDayNumber++;
        }
    }

    public void ChangeState(GameState newState)
    {
        CurrentGameState = newState;

        switch (newState)
        {
            case GameState.Setup:
                Debug.Log("Game is setting up.");
                OnSetupDay?.Invoke();
                SetupDay();
                break;
            case GameState.Starting:
                Debug.Log("Game is starting.");
                StartDay();
                OnDayStarted?.Invoke();
                break;
            case GameState.Running:
                Debug.Log("Game is now running.");
                StartCoroutine(SpawnCallCoroutine());
                StartCoroutine(AddTrashCoroutine());
                OnDayRunning?.Invoke();
                break;
            case GameState.Overtime:
                Debug.Log("Game is now on overtime");
                break;
            case GameState.Paused:
                Debug.Log("Game is now paused.");
                OnDayPaused?.Invoke();
                break;
            case GameState.EndOfDay:
                Debug.Log("Day has ended.");
                OnDayEnded?.Invoke();
                StartCoroutine(ChangeStateDelay(GameState.Cutscene, 1.5f));
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
                if (populationModel.GetPopularityLevel() == PopularityLevel.Bad)
                {
                    Debug.Log("Choosing bad end day.");
                    // Set the current day config to the bad end day
                }
                else
                {
                    Debug.Log("Choosing good end day.");
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
        StartCoroutine(ChangeStateDelay(GameState.Starting, 2f));
    }

    private void StartDay()
    {
        if (currentDayConfig != null)
        {
            currentDayNumber = currentDayConfig.DayNumber;
            currentDayDuration = currentDayConfig.dayDurationInSeconds;
            StartCoroutine(ChangeStateDelay(GameState.Running, 1.5f));
        }
        else
        {
            Debug.LogError("No DayConfigData assigned to GameManager.");
        }
    }

    public void TryEndDay()
    {
        if (CurrentGameState != GameState.Overtime ||
            currentCallsActive > 0) 
            return;

        endOfDayRequested = true;
    }

    public void AdjustActiveCalls(int adjustment)
    {
        currentCallsActive += adjustment;
    }

    private IEnumerator ChangeStateDelay(GameState newState, float delay)
    {
        yield return new WaitForSeconds(delay);
        ChangeState(newState);
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

    private IEnumerator AddTrashCoroutine()
    {
        while (CurrentGameState == GameState.Running)
        {
            float spawnInterval = UnityEngine.Random.Range(currentDayConfig.minTrashAddInterval, currentDayConfig.maxTrashAddInterval);
            Zone randomZone = (Zone)UnityEngine.Random.Range(0, Enum.GetNames(typeof(Zone)).Length);

            yield return new WaitForSeconds(spawnInterval);

            if (CurrentGameState == GameState.Running)
            {
                AddTrash?.Invoke(randomZone);
            }
        }
    }
}
