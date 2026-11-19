using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI fullScreenCheck;
    [SerializeField] private CanvasGroup panelGroup;

    [Header("Volume Sliders")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider voiceSlider;

    private void Awake()
    {
        musicSlider.onValueChanged.AddListener(ChangeMusicVolume);
        sfxSlider.onValueChanged.AddListener(ChangeSFXVolume);
        voiceSlider.onValueChanged.AddListener(ChangeVoiceVolume);
    }

    private void Start()
    {
        RefreshUI();
    }

    public void ToggleFullscreen()
    {
        SettingsManager.Instance.ToggleFullscreen();
        UpdateCheck();
    }

    public void ChangeMusicVolume(float value)
    {
        SettingsManager.Instance.SetMusicVolume(value);
    }

    public void ChangeSFXVolume(float value)
    {
        SettingsManager.Instance.SetSFXVolume(value);
    }

    public void ChangeVoiceVolume(float value)
    {
        SettingsManager.Instance.SetVoiceVolume(value);
    }

    public void OpenPanel()
    {
        RefreshUI();

        panelGroup.alpha = 1;
        panelGroup.blocksRaycasts = true;
        panelGroup.interactable = true;
    }

    public void ClosePanel()
    {
        panelGroup.alpha = 0;
        panelGroup.blocksRaycasts = false;
        panelGroup.interactable = false;

        SettingsManager.Instance.SaveSettings();
    }

    private void RefreshUI()
    {
        SettingsManager settings = SettingsManager.Instance;

        musicSlider.SetValueWithoutNotify(settings.MusicVolume);
        sfxSlider.SetValueWithoutNotify(settings.SFXVolume);
        voiceSlider.SetValueWithoutNotify(settings.VoiceVolume);

        UpdateCheck();
    }

    private void UpdateCheck()
    {
        fullScreenCheck.text = SettingsManager.Instance.IsFullscreen ? "X" : "";
    }

    private void OnDestroy()
    {
        musicSlider.onValueChanged.RemoveListener(ChangeMusicVolume);
        sfxSlider.onValueChanged.RemoveListener(ChangeSFXVolume);
        voiceSlider.onValueChanged.RemoveListener(ChangeVoiceVolume);
    }
}