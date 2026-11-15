using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PopularityAlertView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Sprite negativeSprite;
    [SerializeField] private Sprite positiveSprite;
    
    public RectTransform ObjectTransform { get; private set; }

    private Image currentSprite;
    private CanvasGroup group;
    private Tween moving;
    private Coroutine leaveTimer;
    private bool isLeaving = false;

    void Awake()
    {
        currentSprite = GetComponent<Image>();
        ObjectTransform = GetComponent<RectTransform>();
        group = GetComponent<CanvasGroup>();
    }

    void OnDisable()
    {
        if (leaveTimer != null)
        {
            StopCoroutine(leaveTimer);
            leaveTimer = null;
        }

        moving.Kill();
        ResetAlert();
    }

    public void ConfigureAlert(bool isPositive)
    {
        currentSprite.sprite = isPositive ? positiveSprite : negativeSprite;
    }

    public void AnimateEntry(float newX)
    {
        ObjectTransform.DOAnchorPosX(newX, 0.3f).SetEase(Ease.OutCubic);

        leaveTimer = StartCoroutine(LeaveTimer());
    }

    public void AnimateLeave()
    {
        isLeaving = true;

        moving?.Kill();

        ObjectTransform.DOAnchorPosY(ObjectTransform.anchoredPosition.y + 50, 0.3f);
        group.DOFade(0, 0.3f).OnComplete(() =>
        {
            EventBus.OnAlertExpire?.Invoke(this);
        });
    }

    public void MoveTo(float newY)
    {
        if (isLeaving) return; 

        moving?.Kill();

        moving = ObjectTransform.DOAnchorPosY(newY, 0.3f).SetEase(Ease.OutCubic);
    }

    public void ResetAlert()
    {
        isLeaving = false;

        group.alpha = 1f;

        ObjectTransform.anchoredPosition = new(0, ObjectTransform.anchoredPosition.y);
    }

    private IEnumerator LeaveTimer()
    {
        yield return new WaitForSeconds(2f);

        leaveTimer = null;
        AnimateLeave();
    }
}
