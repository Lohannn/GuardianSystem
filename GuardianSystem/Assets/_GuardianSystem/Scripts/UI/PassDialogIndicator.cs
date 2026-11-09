using UnityEngine;
using DG.Tweening;

public class PassDialogIndicator : MonoBehaviour
{
    [Header("Animation Config")]
    [SerializeField] private float floatHeight;
    [SerializeField] private float cycleDuration;

    [Header("References")]
    [SerializeField] RectTransform rTransform;

    private void OnEnable()
    {
        if (rTransform == null) return;

        rTransform.DOAnchorPosY(rTransform.anchoredPosition.y + floatHeight, cycleDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void OnDisable()
    {
        rTransform.DOKill();
    }
}
