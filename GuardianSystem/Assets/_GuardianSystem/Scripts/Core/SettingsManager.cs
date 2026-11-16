using UnityEngine;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance {  get; private set; }

    [Header("References")]
    [SerializeField] private AudioMixer mixer;

    private const string MusicKey = "MusicVolume";
    private const string SFXKey = "SFXVolume";
    private const string FullscreenKey = "Fullscreen";

    private const string MusicParameter = "MusicVolume";
    private const string SFXParameter = "SFXVolume";

    public float MusicVolume { get; private set; }
    public float SFXVolume { get; private set; }
    public bool IsFullscreen { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        MusicVolume = PlayerPrefs.GetFloat(MusicKey, 1f);
        SFXVolume = PlayerPrefs.GetFloat(SFXKey, 1f);

        IsFullscreen = PlayerPrefs.GetInt(
            FullscreenKey,
            Screen.fullScreen ? 1 : 0
        ) == 1;
    }

    private void Start()
    {
        ApplyVolume(MusicParameter, MusicVolume);
        ApplyVolume(SFXParameter, SFXVolume);

        ApplyFullscreen();
    }

    public void SetMusicVolume(float value)
    {
        MusicVolume = Mathf.Clamp01(value);
        ApplyVolume(MusicParameter, MusicVolume);

        PlayerPrefs.SetFloat(MusicKey, MusicVolume);
    }

    public void SetSFXVolume(float value)
    {
        SFXVolume = Mathf.Clamp01(value);
        ApplyVolume(SFXParameter, SFXVolume);

        PlayerPrefs.SetFloat(SFXKey, SFXVolume);
    }

    private void ApplyVolume(string parameter, float value)
    {
        float decibels = value <= 0f
            ? -80f
            : Mathf.Log10(value) * 20f;

        mixer.SetFloat(parameter, decibels);
    }

    public void ToggleFullscreen()
    {
        IsFullscreen = !IsFullscreen;

        ApplyFullscreen();

        PlayerPrefs.SetInt(
            FullscreenKey,
            IsFullscreen ? 1 : 0
        );
    }

    private void ApplyFullscreen()
    {
        if (IsFullscreen)
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
    }

    public void SaveSettings()
    {
        PlayerPrefs.Save();
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused)
            SaveSettings();
    }

    private void OnApplicationQuit()
    {
        SaveSettings();
    }
}
