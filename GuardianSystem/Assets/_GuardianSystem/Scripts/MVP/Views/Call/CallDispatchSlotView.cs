using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CallDispatchSlotView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CallDispatchSlotPresenter presenter;
    [SerializeField] private Image unitIcon;

    [SerializeField] private Image outerInactiveSquare;
    [SerializeField] private Image innerInactiveSquare;
    [SerializeField] private RectTransform innerSquareTransform;

    [Header("Animation Config")]
    [SerializeField] private float colorChangeDuration;
    [SerializeField] private float innerScaleChangeDuration;
    [SerializeField] private float activeInnerOffset;
    private float defaultInnerOffset;

    void Awake()
    {
        defaultInnerOffset = innerSquareTransform.offsetMin.x;
    }

    void OnDisable()
    {
        RemoveIcon();
    }

    public void SetIcon(UnitSO unit)
    {
        unitIcon.sprite = unit.icon;
        unitIcon.color = new(unitIcon.color.r, unitIcon.color.g, unitIcon.color.b, 1f);

        ActiveSlotAnimation();
    }

    public void RemoveIcon()
    {
        unitIcon.sprite = null;
        unitIcon.color = new(unitIcon.color.r, unitIcon.color.g, unitIcon.color.b, 0f);

        ResetVisual();
    }

    private void ActiveSlotAnimation()
    {
        outerInactiveSquare.DOFade(0, colorChangeDuration);
        innerInactiveSquare.DOFade(0, colorChangeDuration);

        DOOffsets(activeInnerOffset, innerScaleChangeDuration);
    }

    private void ResetVisual()
    {
        outerInactiveSquare.DOFade(1, colorChangeDuration);
        innerInactiveSquare.DOFade(1, colorChangeDuration);

        DOOffsets(defaultInnerOffset, innerScaleChangeDuration);
    }

    private void DOOffsets(float endValue, float duration)
    {
        DOTween.To(
            () => innerSquareTransform.offsetMin,
            x => innerSquareTransform.offsetMin = x,
            new Vector2(endValue, endValue),
            duration
        ).SetEase(Ease.InOutBack, 6);

        DOTween.To(
            () => innerSquareTransform.offsetMax,
            x => innerSquareTransform.offsetMax = x,
            new Vector2(-endValue, -endValue),
            duration
        ).SetEase(Ease.InOutBack, 6);
    }
}
