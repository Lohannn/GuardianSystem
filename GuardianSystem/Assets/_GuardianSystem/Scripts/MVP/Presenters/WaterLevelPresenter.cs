using System.Collections.Generic;
using UnityEngine;

public class WaterLevelPresenter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WaterLevelView view;
    private GameManager gameManager;

    [Header("Water level Config")]
    private WaterLevelModel waterLevel;

    void Awake()
    {
        gameManager = GameManager.Instance;
    }

    void OnEnable()
    {
        gameManager.OnDayStarted += CreateNewWaterLevels;
    }

    void OnDisable()
    {
        gameManager.OnDayStarted -= CreateNewWaterLevels;
    }

    void Update()
    {
        if (gameManager.CurrentGameState == GameState.Running)
        {
            UpdateValues(Time.deltaTime);
        }
    }

    private void UpdateValues(float deltaTime)
    {
        Dictionary<Zone, float> updatedValues = waterLevel.UpdateValues(deltaTime);

        view.UpdateSliders(updatedValues);
    }

    public void ChangeZoneLevel(Zone zone, float newValue)
    {
        waterLevel.SetZoneLevel(zone, newValue);
    }

    private void CreateNewWaterLevels()
    {
        waterLevel = new WaterLevelModel(gameManager);
    }
}
