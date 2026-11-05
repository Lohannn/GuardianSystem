using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ZonePresenter : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    [SerializeField] private ZoneView view;

    [Header("Zone Info")]
    [SerializeField] private Zone zone;
    private int trashCounter;
    private float penaltyInterval;
    private float penaltyTimer;
     
    public Zone Zone => zone;
    public CallButtonPresenter currentCall;

    private UnitItemPresenter collectorUnit;

    private GameManager gameManager;

    void Awake()
    {
        gameManager = GameManager.Instance;
    }

    void OnEnable()
    {
        EventBus.OnCallResolved += FreeWaterCallPosition;
        EventBus.OnCallTimedOut += FreeWaterCallPosition;

        gameManager.OnDayStarted += SetupTrash;
        gameManager.AddTrash += AddTrashCounter;
        gameManager.OnDayEnded += ResetTranshCounter;
    }

    void OnDisable()
    {
        EventBus.OnCallResolved -= FreeWaterCallPosition;
        EventBus.OnCallTimedOut -= FreeWaterCallPosition;

        gameManager.OnDayStarted -= SetupTrash;
        gameManager.AddTrash -= AddTrashCounter;
        gameManager.OnDayEnded -= ResetTranshCounter;
    }

    private void SetupTrash()
    {
        trashCounter = 0;
        penaltyInterval = gameManager.currentDayConfig.fullTrashTickInterval;
        penaltyTimer = 0;
    }

    void Update()
    {
        if (gameManager.CurrentGameState == GameState.Running)
        {
            if (trashCounter < 4)
            {
                penaltyTimer = 0;
                return;
            }

            penaltyTimer += Time.deltaTime;

            if (penaltyTimer >= penaltyInterval)
            {
                gameManager.populationModel.AdjustPopularity(gameManager.currentDayConfig.popularityImpactOnFullTrashPerTick);
                penaltyTimer = 0;
            }
        }
    }

    private void FreeWaterCallPosition(EventData data, CallButtonPresenter call)
    {
        if (call != currentCall) return;

        currentCall = null;
    }

    private void AddTrashCounter(Zone requisitedZone)
    {
        if (requisitedZone != zone || trashCounter >= 4)
        {
            return;
        }

        trashCounter++;
        view.UpdateIcons(trashCounter);
    }

    private void CollectTrashCounter()
    {
        gameManager.populationModel.AdjustPopularity(gameManager.currentDayConfig.popularityImpactOnCollectedTrash * trashCounter);
        trashCounter = 0;
        collectorUnit = null;
        view.UpdateIcons(trashCounter);
    }

    private void ResetTranshCounter()
    {
        if (collectorUnit != null)
        {
            return;
        }

        trashCounter = 0;
        collectorUnit = null;
        view.UpdateIcons(trashCounter);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (collectorUnit != null || trashCounter <= 0 || 
            gameManager.CurrentGameState != GameState.Running &&
            gameManager.CurrentGameState != GameState.Overtime) return;

        UnitItemPresenter droppedUnit = eventData.pointerDrag?.GetComponent<UnitItemPresenter>();

        if (droppedUnit == null || droppedUnit.Unit.type != UnitType.TrashCollector) return;

        collectorUnit = droppedUnit;

        EventBus.OnDispatchStart?.Invoke(collectorUnit, 
                                         UnitMovementCalculator.CalculateTravelTime(collectorUnit.UnitBase.position,
                                                                                    transform.position,
                                                                                    collectorUnit.Unit.speedBySecond),
                                         CollectTrashCounter);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!eventData.pointerDrag || trashCounter <= 0 || 
            gameManager.CurrentGameState != GameState.Running &&
            gameManager.CurrentGameState != GameState.Overtime) return;

        UnitItemPresenter unitPreview = eventData.pointerDrag.GetComponent<UnitItemPresenter>();

        if (unitPreview == null || unitPreview.Unit.type != UnitType.TrashCollector) return;

        EventBus.OnShowDispatchPreview?.Invoke(UnitMovementCalculator.CalculateTravelTime(
                                               unitPreview.UnitBase.position, 
                                               transform.position,
                                               unitPreview.Unit.speedBySecond));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!eventData.pointerDrag) return;

        EventBus.OnHideDispatchPreview?.Invoke();
    }
}
