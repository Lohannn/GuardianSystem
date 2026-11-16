using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

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
        EventBus.OnFadeOut += FadeOut;
        EventBus.OnFadeIn += FadeIn;
    }

    void OnDisable()
    {
        EventBus.OnFadeOut -= FadeOut;
        EventBus.OnFadeIn -= FadeIn;
    }

    private void FadeOut(Action onCompleteAction)
    {
        AnimateFade(1f, false, onCompleteAction);
    }

    private void FadeIn(Action onCompleteAction)
    {
        AnimateFade(-0.5f, true, onCompleteAction);
    }

    private void AnimateFade(float targetValue, bool isRevealing, Action onCompleteAction)
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

            onCompleteAction?.Invoke();
        });
    }
}