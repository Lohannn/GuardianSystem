using DG.Tweening;
using TMPro;
using UnityEngine;

public class MomentTitleView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform overtimeTitle;
    [SerializeField] private CanvasGroup dayTitleGroup;
    [SerializeField] private TextMeshProUGUI dayTitle;

    [Header("Overtime Animation Config")]
    [SerializeField] private float entranceDuration;
    [SerializeField] private float holdDuration;
    [SerializeField] private float exitDuration;

    [Header("Day Title Animation Config")]
    [SerializeField] private float showAnimDuration;
    [SerializeField] private float leaveAnimDuration;

    private Sequence overtimeSequence;

    void OnEnable()
    {
        GameManager.Instance.OnDayOvertime += ShowOvertimeTitle;
        GameManager.Instance.OnDayStarted += HideDay;
        
        EventBus.OnShowDayTitle += ShowDayTitle;
    }

    void OnDisable()
    {
        GameManager.Instance.OnDayOvertime -= ShowOvertimeTitle;
        GameManager.Instance.OnDayStarted -= HideDay;
        
        EventBus.OnShowDayTitle -= ShowDayTitle;
    }

    private void ShowDayTitle(int dayNumber)
    {
        dayTitle.text = $"Day {dayNumber}";
        dayTitleGroup.DOFade(1, showAnimDuration);
    }

    private void HideDay()
    {
        dayTitleGroup.DOFade(0, leaveAnimDuration);
    }

    private void ShowOvertimeTitle()
    {
        float leftOffscreen = -1500;
        float rightOffscreen = 1500;

        overtimeTitle.anchoredPosition = new(leftOffscreen, overtimeTitle.anchoredPosition.y);

        overtimeSequence = DOTween.Sequence()
            .Append(
                overtimeTitle.DOAnchorPosX(0, entranceDuration).SetEase(Ease.OutCubic)
            )
            .AppendInterval(holdDuration)
            .Append(
                overtimeTitle.DOAnchorPosX(rightOffscreen, entranceDuration).SetEase(Ease.OutCubic)
            );
    }
}
