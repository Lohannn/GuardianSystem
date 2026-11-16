using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private MusicListSO musicList;
    [SerializeField] private SFXListSO sfxList;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Fade Config")]
    [SerializeField] private float fadeDuration;

    [Header("Music Speed")]
    [SerializeField] private AudioMixer musicMixer;

    private const string PitchCorrection = "PitchCorrection";

    private Tween musicRushTween;
    private Tween musicFadeTween;
    private float defaultMusicVolume;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        defaultMusicVolume = musicSource.volume;
        musicSource.loop = true;
    }

    public void PlaySFX(AudioClip audio)
    {
        sfxSource.pitch = Random.Range(0.95f, 1.05f);
        sfxSource.PlayOneShot(audio);
    }

    public void PlayMusic(AudioClip audio)
    {
        if (audio == null) return;

        ResetMusicSpeed();

        musicFadeTween?.Kill();

        musicSource.volume = 0f;

        if (musicSource.clip == audio && musicSource.isPlaying)
        {
            musicFadeTween = musicSource
                .DOFade(defaultMusicVolume, fadeDuration)
                .SetUpdate(true);

            return;
        }

        Sequence sequence = DOTween.Sequence().SetUpdate(true);

        if (musicSource.isPlaying)
        {
            sequence.Append(
                musicSource.DOFade(0f, fadeDuration)
            );
        }

        sequence.AppendCallback(() =>
        {
            musicSource.Stop();
            musicSource.clip = audio;
            musicSource.volume = 0f;
            musicSource.Play();
        });

        sequence.Append(
            musicSource.DOFade(defaultMusicVolume, fadeDuration)
        );

        musicFadeTween = sequence;
    }

    private void SetMusicSpeed(float speed)
    {
        musicSource.pitch = speed;
        musicMixer.SetFloat(PitchCorrection, 1f / speed);
    }

    public void StartMusicRush(float duration = 30f, float maxSpeed = 1.2f)
    {
        musicRushTween?.Kill();

        musicRushTween = DOTween.To(
            () => musicSource.pitch,
            speed => SetMusicSpeed(speed),
            maxSpeed,
            duration
        ).SetEase(Ease.InQuad);
    }

    public void ResetMusicSpeed()
    {
        musicRushTween?.Kill();
        SetMusicSpeed(1f);
    }

    public void StopMusicWithoutFade()
    {
        ResetMusicSpeed();

        musicFadeTween?.Kill();

        musicSource.Stop();
    }

    public void StopMusic()
    {
        ResetMusicSpeed();

        musicFadeTween?.Kill();

        if (!musicSource.isPlaying) return;

        musicFadeTween = musicSource
            .DOFade(0f, fadeDuration)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                musicSource.Stop();
                musicSource.volume = defaultMusicVolume;
            });
    }

    public AudioClip GetSFX(SFXType type)
    {
        return sfxList.GetSFX(type);
    }

    public AudioClip GetMusic(MusicType type)
    {
        return musicList.GetMusic(type);
    }
}
