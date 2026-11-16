using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class IntroViiew : MonoBehaviour
{
     [Header("References")]
    [SerializeField] private CanvasGroup introContainer;
    [SerializeField] private Image blackCover;

    [SerializeField] private Image topLine;
    [SerializeField] private Image bottomLine;

    [SerializeField] private RectTransform introLogo;
    [SerializeField] private CanvasGroup logoGroup;

    [SerializeField] private RectTransform pulseRing;
    [SerializeField] private CanvasGroup pulseGroup;

    [Header("Animation Config")]
    [SerializeField] private float linesDuration = 0.7f;
    [SerializeField] private float pulseDuration = 0.5f;
    [SerializeField] private float revealDuration = 1f;

    private Sequence introSequence;
    private Vector3 logoOriginalScale;

    private void Awake()
    {
        logoOriginalScale = introLogo.localScale;

        PrepareIntro();
    }

    private void Start()
    {
        StartCoroutine(PlayIntroDelay());
    }

    private void PrepareIntro()
    {
        introContainer.blocksRaycasts = true;

        blackCover.color = Color.black;
        blackCover.raycastTarget = true;

        topLine.fillAmount = 0f;
        bottomLine.fillAmount = 0f;

        logoGroup.alpha = 0f;
        introLogo.localScale = logoOriginalScale * 0.7f;

        pulseGroup.alpha = 0f;
        pulseRing.localScale = Vector3.one * 0.15f;
    }

    public void PlayIntro()
    {
        introSequence?.Kill();
        PrepareIntro();

        introSequence = DOTween.Sequence()
            .SetUpdate(true);

        introSequence.Append(
            topLine.DOFillAmount(1f, linesDuration)
                .SetEase(Ease.OutCubic)
        );

        introSequence.Join(
            bottomLine.DOFillAmount(1f, linesDuration)
                .SetEase(Ease.OutCubic)
        );

        introSequence.AppendInterval(0.15f);

        introSequence.Append(
            logoGroup.DOFade(1f, 0.15f)
        );

        introSequence.Join(
            introLogo.DOScale(
                logoOriginalScale * 1.15f, 0.15f
            ).SetEase(Ease.OutCubic)
        );

        introSequence.JoinCallback(() =>
        {
            pulseRing.localScale = Vector3.one * 0.15f;
            pulseGroup.alpha = 1f;
        });

        introSequence.Join(
            pulseRing.DOScale(16f, pulseDuration)
                .SetEase(Ease.OutCubic)
        );

        introSequence.Join(
            introLogo.DOScale(
                logoOriginalScale, 0.2f
            ).SetEase(Ease.OutBack)
        );

        introSequence.Join(
            pulseGroup.DOFade(0f, pulseDuration)
                .SetEase(Ease.InQuad)
        );

        introSequence.Join(
            topLine.DOFillAmount(0f, pulseDuration)
                .SetEase(Ease.OutCubic)
        );

        introSequence.Join(
            bottomLine.DOFillAmount(0f, pulseDuration)
                .SetEase(Ease.OutCubic)
        );

        introSequence.AppendInterval(0.1f);

        introSequence.Append(
            blackCover.DOFade(0f, revealDuration)
                .SetEase(Ease.InOutSine)
        );

        introSequence.JoinCallback(() =>
        {
            introContainer.blocksRaycasts = false;
        });

        introSequence.Join(
            logoGroup.DOFade(0f, revealDuration)
        );
    }

    private IEnumerator PlayIntroDelay()
    {
        yield return new WaitForSeconds(1f);

        PlayIntro();
    }
}
