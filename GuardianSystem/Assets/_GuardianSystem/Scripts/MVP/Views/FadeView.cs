using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FadeView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image fadeImage;

    [Header("Fade Animation Config")]
    [SerializeField] private float duration;
    [SerializeField] private float softness;

    private Material material;

    private static readonly int Progress = Shader.PropertyToID("_Progress");
    private static readonly int Softness = Shader.PropertyToID("_Softness");
    private static readonly int Direction = Shader.PropertyToID("_Direction");

    void Awake()
    {
        material = fadeImage.material;
        material.SetFloat(Softness, softness);
        material.SetFloat(Progress, -0.5f);

        fadeImage.raycastTarget = false;
    }

    void OnEnable()
    {
        EventBus.OnFadeIn += FadeIn;
        EventBus.OnFadeOut += FadeOut;
    }

    private void FadeIn()
    {
        AnimateFade(1f, false);
    }

    private void FadeOut()
    {
        AnimateFade(-0.5f, true);
    }

    private void AnimateFade(float targetValue, bool isRevealing)
    {
        fadeImage.raycastTarget = true;

        material.SetFloat(Direction, isRevealing ? 1 : 0);

        DOTween.To(
            () => material.GetFloat(Progress),
            value => material.SetFloat(Progress, value),
            targetValue,
            duration
        ).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            fadeImage.raycastTarget = targetValue > 0f;
        });
    }
}