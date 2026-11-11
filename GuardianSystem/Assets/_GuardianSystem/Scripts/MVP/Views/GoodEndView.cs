using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class GoodEndView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<TextMeshProUGUI> creditsSequence;
    [SerializeField] private CanvasGroup goodEndImageGroup;

    [Header("Animation Config")]
    [SerializeField, Range(0.05f, 0.45f)]
    private float fadePercentage;
    [SerializeField] private float freeCreditsTime;
    [SerializeField] private float finalThanksFadeDuration;

    private Sequence creditsTween;

    private void OnEnable()
    {
        GameManager.Instance.OnUtopiaRunning += StartCredits;
        GameManager.Instance.EndUtopiaDay += EndGoodEnding;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnUtopiaRunning -= StartCredits;
        GameManager.Instance.EndUtopiaDay -= EndGoodEnding;
        creditsTween.Kill();
    }

    private void StartCredits()
    {
        if (creditsSequence.Count == 0)
        {
            Debug.LogError("No credits to show.");
            return;
        }

        creditsTween?.Kill();

        float dayDuration = GameManager.Instance.currentDayConfig.dayDurationInSeconds;

        float timePerCredit = (dayDuration - freeCreditsTime) / creditsSequence.Count;

        float fadeDuration = timePerCredit * fadePercentage;
        float visibleDuration = timePerCredit - (fadeDuration * 2f);

        creditsTween = DOTween.Sequence();

        foreach (TextMeshProUGUI text in creditsSequence)
        {
            creditsTween.Append(
                text.DOFade(1f, fadeDuration)
            );

            creditsTween.AppendInterval(visibleDuration);

            creditsTween.Append(
                text.DOFade(0f, fadeDuration)
            );
        }
    }

    private void EndGoodEnding()
    {
        goodEndImageGroup.DOFade(1f, finalThanksFadeDuration)
            .OnComplete(() =>
            {
                StartCoroutine(GoToMainMenu());
            });
    }

    private IEnumerator GoToMainMenu()
    {
        yield return new WaitForSeconds(2f);

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}