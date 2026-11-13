using TMPro;
using UnityEngine;

public class SettingsPanel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI fullScreenCheck;
    [SerializeField] private CanvasGroup panelGroup;

    private bool inFullscreen;

    private void Start()
    {
        inFullscreen = Screen.fullScreen;

        UpdateCheck();
    }

    public void ToggleFullscreen()
    {
        inFullscreen = !inFullscreen;

        if (inFullscreen)
        {
            Screen.SetResolution(
                Screen.currentResolution.width,
                Screen.currentResolution.height,
                FullScreenMode.FullScreenWindow
            );
        }
        else
        {
            Screen.SetResolution(
                1280,
                720,
                FullScreenMode.Windowed
            );
        }

        UpdateCheck();
    }

    public void OpenPanel()
    {
        panelGroup.alpha = 1;
        panelGroup.blocksRaycasts = true;
        panelGroup.interactable = true;
    }

    public void ClosePanel()
    {
        panelGroup.alpha = 0;
        panelGroup.blocksRaycasts = false;
        panelGroup.interactable = false;
    }

    private void UpdateCheck()
    {
        fullScreenCheck.text = inFullscreen ? "X" : "";
    }
}