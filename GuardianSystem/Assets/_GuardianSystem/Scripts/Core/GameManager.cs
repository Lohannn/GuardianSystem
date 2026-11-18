using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Setup,
    Starting,
    Interrupted,
    Utopia,
    Running,
    Overtime,
    Paused,
    EndOfDay,
    Cutscene
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private UIDigitalReveal dayReveal;

    [Header("Game State manager")]
    public GameState CurrentGameState { get; private set; } = GameState.Starting;
    private int currentCallsActive = 0;

    [Header("Day configuration")]
    [SerializeField] private List<DayConfigData> dayConfigOrder;
    [SerializeField] private DayConfigData badEndConfig;
    [SerializeField] private DayConfigData goodEndConfig;
    public DayConfigData currentDayConfig;
    [NonSerialized] public PopulationModel populationModel;
    private PlayerData playerData;
    public static bool ContinueRequested { get; private set; }

    [Header("Day Data")]
    public int currentDayNumber = 0;
    private float currentDayDuration;
    private bool endOfDayRequested = false;
    private bool musicRushStarted = false;

    //Events
#region Game State Events
    public event Action OnSetupDay;
    public event Action OnDayStarted;
    public event Action OnDayInterrupted;
    public event Action OnUtopiaRunning;
    public event Action OnDayRunning;
    public event Action OnDayOvertime;
    public event Action OnDayEnded;
    public event Action OnCutsceneStarted;
#endregion

    public event Action SpawnCall;
    public event Action<Zone> AddTrash;
    public event Action EndUtopiaDay;

    public bool utopiaDay = false;

    public static void SetContinueRequested(bool value)
    {
        ContinueRequested = value;
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        populationModel = new PopulationModel();
        ChangeState(GameState.Setup);
    }

    private void Update()
    {
        if (CurrentGameState == GameState.Running || CurrentGameState == GameState.Utopia)
        {
            currentDayDuration -= Time.deltaTime;

            if (CurrentGameState != GameState.Utopia && !musicRushStarted && currentDayDuration <= 30f)
            {
                musicRushStarted = true;
                AudioManager.Instance.StartMusicRush(maxSpeed:1.8f);
            }

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
                if (currentDayNumber > 0 && !ContinueRequested) dayReveal.PrepareReveal();
                break;
            case GameState.Starting:
                Debug.Log("Game is starting.");
                StartDay();
                OnDayStarted?.Invoke();
                EventBus.OnFadeIn?.Invoke(null);
                if (currentDayNumber > 0 && !ContinueRequested) dayReveal.PlayReveal();
                ContinueRequested = false;
                break;
            case GameState.Interrupted:
                Debug.Log("Game has been interrupted.");
                AudioManager.Instance.StopMusicWithoutFade();
                OnDayInterrupted?.Invoke();
                break;
            case GameState.Utopia:
                Debug.Log("Game has been entered in a Utopia");
                utopiaDay = true;
                StartCoroutine(SpawnCallCoroutine());
                OnUtopiaRunning?.Invoke();
                break;
            case GameState.Running:
                Debug.Log("Game is now running.");
                StartCoroutine(SpawnCallCoroutine());
                StartCoroutine(AddTrashCoroutine());
                OnDayRunning?.Invoke();
                break;
            case GameState.Overtime:
                Debug.Log("Game is now on overtime");

                if (!utopiaDay)
                {
                    OnDayOvertime?.Invoke();
                    AudioManager.Instance.StopMusicWithoutFade();
                }

                if (utopiaDay) EndUtopiaDay?.Invoke();
                break;
            case GameState.EndOfDay:
                Debug.Log("Day has ended.");
                OnDayEnded?.Invoke();
                EventBus.OnFadeOut?.Invoke(null);
                StartCoroutine(ChangeStateDelay(GameState.Cutscene, 3f));
                break;
            case GameState.Cutscene:
                Debug.Log("Cutscene is now playing.");
                OnCutsceneStarted?.Invoke();
                EventBus.OnFadeIn?.Invoke(null);
                break;
        }
    }

    private void SetupDay()
    {
        //Save
        if (playerData == null)
        {
            playerData = SaveData.LoadGame();

            if (ContinueRequested)
            {
                currentDayNumber = playerData.lastDayNumber;
                populationModel.SetPopularity(playerData.lastPopularityValue);
            }
        }

        if (currentDayConfig == badEndConfig || currentDayConfig == goodEndConfig)
            return;

        if (dayConfigOrder == null || dayConfigOrder.Count == 0)
        {
            Debug.LogError("No day config available.");
            return;
        }

        if (currentDayNumber < 0 || currentDayNumber > dayConfigOrder.Count)
        {
            Debug.LogError($"{currentDayNumber} isn't a invalid Day number.");
            return;
        }

        if (currentDayNumber > 0)
        {
            SaveData.currentPlayerData.lastDayNumber = currentDayNumber;
            SaveData.currentPlayerData.lastPopularityValue = populationModel.Popularity;

            SaveData.SaveGame();
        }

        currentDayConfig = currentDayNumber == dayConfigOrder.Count
            ? GetEndingDay()
            : dayConfigOrder[currentDayNumber];

        musicRushStarted = false;
        AudioManager.Instance.ResetMusicSpeed();

        Debug.Log($"Setting up Day {currentDayConfig.name}.");
        StartCoroutine(ChangeStateDelay(GameState.Starting, 2f));
    }

    private void StartDay()
    {
        if (currentDayConfig != null)
        {
            currentDayDuration = currentDayConfig.dayDurationInSeconds;

            GameState nextState = currentDayConfig == goodEndConfig 
                ? GameState.Utopia 
                : GameState.Running;

            StartCoroutine(ChangeStateDelay(nextState, 1.5f));

            AudioManager.Instance.PlayMusic(nextState == GameState.Utopia 
                                                         ? AudioManager.Instance.GetMusic(MusicType.MainMusic)
                                                         : AudioManager.Instance.GetMusic(MusicType.Stage));

            print($"Is bad ending?: {currentDayConfig == badEndConfig}");
            if (currentDayConfig == badEndConfig)
            {
                StartCoroutine(BadEndInterruption());
            }
        }
        else
        {
            Debug.LogError("No DayConfigData assigned to GameManager.");
        }
    }

    public void TryEndDay()
    {
        if (CurrentGameState != GameState.Overtime ||
            currentCallsActive > 0 || utopiaDay) 
            return;

        endOfDayRequested = true;
    }

    private DayConfigData GetEndingDay()
    {
        bool isBadEnding = populationModel.GetPopularityLevel() == PopularityLevel.Bad;

        Debug.Log(isBadEnding ? "Choosing Bad End day." : "Choosing Good End day");

        return isBadEnding ? badEndConfig : goodEndConfig;
    }

    public void AdjustActiveCalls(int adjustment)
    {
        currentCallsActive += adjustment;
    }

    public IEnumerator ChangeStateDelay(GameState newState, float delay)
    {
        yield return new WaitForSeconds(delay);
        ChangeState(newState);
    }

    private IEnumerator BadEndInterruption()
    {
        yield return new WaitForSeconds(20f);
        ChangeState(GameState.Interrupted);
    }

    private IEnumerator SpawnCallCoroutine()
    {
        while (CurrentGameState == GameState.Running || CurrentGameState == GameState.Utopia)
        {
            if (currentCallsActive >= currentDayConfig.maxSimultaneousCalls)
            {
                yield return new WaitForSeconds(0.5f);
                continue;
            }

            float spawnInterval = UnityEngine.Random.Range(currentDayConfig.minCallSpawnInterval, currentDayConfig.maxCallSpawnInterval);

            yield return new WaitForSeconds(spawnInterval);

            if ((CurrentGameState == GameState.Running || CurrentGameState == GameState.Utopia)
                && currentCallsActive < currentDayConfig.maxSimultaneousCalls)
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
