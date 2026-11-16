using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SettingsPanel pausePanel;
    [SerializeField] private ExtraPanelOpen pausePanelToggle;

    public bool IsPaused { get; private set; } = false;

    private bool isClosing;

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    private void TogglePause()
    {
        if (isClosing) return;

        if (IsPaused)
            Resume();
        else
            Pause();
    }

    public void Pause()
    {
        if (IsPaused || isClosing) return;

        IsPaused = true;
        Time.timeScale = 0f;

        pausePanel.OpenPanel();
        pausePanelToggle.OpenPanelAnimated();
    }

    public void Resume()
    {
        if (!IsPaused || isClosing) return;

        isClosing = true;

        pausePanelToggle.ClosePanelAnimated(() =>
        {
            pausePanel.ClosePanel();

            IsPaused = false;
            isClosing = false;
            Time.timeScale = 1f;
        });
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}