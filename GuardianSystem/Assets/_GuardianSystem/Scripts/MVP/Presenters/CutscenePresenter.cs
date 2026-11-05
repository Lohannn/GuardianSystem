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

        view.SetupCutscene(currentCutscene, 
                           currentCutscene.GetDialogues(gameManager.populationModel.GetPopularityLevel()));
    }

    public void EndCutscene()
    {
        gameManager.ChangeState(GameState.Setup);
    }
}
