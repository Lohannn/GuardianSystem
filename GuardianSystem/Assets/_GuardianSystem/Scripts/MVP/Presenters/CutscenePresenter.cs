using UnityEngine;

public class CutscenePresenter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CutsceneView view;

    private GameManager gameManager;

    void Awake()
    {
        gameManager = GameManager.Instance;
    }

    void OnEnable()
    {
        gameManager.OnCutsceneStarted += StartScene;
    }

    void OnDisable()
    {
        gameManager.OnCutsceneStarted -= StartScene;
    }

    private void StartScene()
    {
        CutsceneData currentCutscene = gameManager.currentDayConfig.endDayCutscene;

        view.SetupCutscene(currentCutscene.GetImage(gameManager.populationModel.GetPopularityLevel()), 
                           currentCutscene.GetDialogues(gameManager.populationModel.GetPopularityLevel()));
    }

    public void EndCutscene()
    {
        EventBus.OnShowDayTitle?.Invoke(gameManager.currentDayNumber + 1);
        StartCoroutine(gameManager.ChangeStateDelay(GameState.Setup, 3f));
    }
}
