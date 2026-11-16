using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIDigitalReveal : MonoBehaviour
{
    [SerializeField] private Image overlayImage;
    [SerializeField] private float duration = 0.8f;

    private Material runtimeMaterial;
    private Tween revealTween;

    private static readonly int Progress = Shader.PropertyToID("_Progress");

    private void Awake()
    {
        runtimeMaterial = new Material(overlayImage.material);
        overlayImage.material = runtimeMaterial;

        overlayImage.raycastTarget = false;
        overlayImage.enabled = false;
    }

    public void PrepareReveal()
    {
        revealTween?.Kill();

        overlayImage.enabled = true;
        runtimeMaterial.SetFloat(Progress, 0f);
    }

    public void PlayReveal()
    {
        revealTween?.Kill();

        if (!overlayImage.enabled)
            PrepareReveal();

        revealTween = DOTween.To(
            () => runtimeMaterial.GetFloat(Progress),
            value => runtimeMaterial.SetFloat(Progress, value),
            1f,
            duration
        )
        .SetEase(Ease.Linear)
        .SetUpdate(true)
        .OnComplete(() =>
        {
            overlayImage.enabled = false;
        });
    }

    private void OnDestroy()
    {
        revealTween?.Kill();

        if (runtimeMaterial != null)
            Destroy(runtimeMaterial);
    }
}
