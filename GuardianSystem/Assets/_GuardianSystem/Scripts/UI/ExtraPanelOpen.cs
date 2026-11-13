using System;
using DG.Tweening;
using UnityEngine;

public class ExtraPanelOpen : MonoBehaviour
{
    [SerializeField] private RectTransform extraPanelContainer;
    [SerializeField] private Vector2 openedSize;

    private bool isOpened = false;

    [Header("Animation config")]
    [SerializeField] private float scaleXChangeDuration;
    [SerializeField] private float scaleYChangeDuration;

    private void OnEnable()
    {
        if(GameManager.Instance == null) return;

        GameManager.Instance.OnSetupDay += ClosePanel;
    }

    private void OnDisable()
    {
        if(GameManager.Instance == null) return;

        GameManager.Instance.OnSetupDay -= ClosePanel;
    }

    public void ClosePanel()
    {
        extraPanelContainer.sizeDelta = new(0, 40);
        isOpened = false;
    }

    public void TogglePanelWindowAnimated(Action action)
    {
        if (isOpened)
        {
            extraPanelContainer.DOSizeDelta(new(openedSize.x, 40), scaleYChangeDuration).SetEase(Ease.InQuad)
                .OnComplete(() =>
                {
                    extraPanelContainer.DOSizeDelta(new(0, 40), scaleXChangeDuration).SetEase(Ease.InSine)
                        .OnComplete(() =>
                        {
                            action?.Invoke();
                        });
                });
        }
        else
        {
            extraPanelContainer.DOSizeDelta(new(openedSize.x, 40), scaleXChangeDuration).SetEase(Ease.OutSine)
                .OnComplete(() =>
                {
                    extraPanelContainer.DOSizeDelta(new(openedSize.x, openedSize.y), scaleYChangeDuration).SetEase(Ease.OutBack)
                        .OnComplete(() =>
                        {
                            action?.Invoke();
                        });
                });
        }

        isOpened = !isOpened;
    }

    public void TogglePanelWindowAnimated()
    {
        if (isOpened)
        {
            extraPanelContainer.DOSizeDelta(new(openedSize.x, 40), scaleYChangeDuration).SetEase(Ease.InQuad)
                .OnComplete(() =>
                {
                    extraPanelContainer.DOSizeDelta(new(0, 40), scaleXChangeDuration).SetEase(Ease.InSine);
                });
        }
        else
        {
            extraPanelContainer.DOSizeDelta(new(openedSize.x, 40), scaleXChangeDuration).SetEase(Ease.OutSine)
                .OnComplete(() =>
                {
                    extraPanelContainer.DOSizeDelta(new(openedSize.x, openedSize.y), scaleYChangeDuration).SetEase(Ease.OutBack);
                });
        }

        isOpened = !isOpened;
    }

    public void OpenPanelAnimated()
    {
        extraPanelContainer.DOSizeDelta(new(openedSize.x, extraPanelContainer.sizeDelta.y), scaleXChangeDuration).SetEase(Ease.OutSine)
                .OnComplete(() =>
                {
                    extraPanelContainer.DOSizeDelta(new(openedSize.x, openedSize.y), scaleYChangeDuration).SetEase(Ease.OutBack);
                });

        isOpened = true;
    }

    public void OpenPanelAnimated(Action action)
    {
        extraPanelContainer.DOSizeDelta(new(openedSize.x, extraPanelContainer.sizeDelta.y), scaleXChangeDuration).SetEase(Ease.OutSine)
                .OnComplete(() =>
                {
                    extraPanelContainer.DOSizeDelta(new(openedSize.x, openedSize.y), scaleYChangeDuration).SetEase(Ease.OutBack)
                        .OnComplete(() =>
                        {
                            action?.Invoke();
                        });
                });

        isOpened = true;
    }

    public void ClosePanelAnimated()
    {
        extraPanelContainer.DOSizeDelta(new(extraPanelContainer.sizeDelta.x, 40), scaleYChangeDuration).SetEase(Ease.InQuad)
                .OnComplete(() =>
                {
                    extraPanelContainer.DOSizeDelta(new(0, extraPanelContainer.sizeDelta.y), scaleXChangeDuration).SetEase(Ease.InSine);
                });

        isOpened = false;
    }

    public void ClosePanelAnimated(Action action)
    {
        extraPanelContainer.DOSizeDelta(new(extraPanelContainer.sizeDelta.x, 40), scaleYChangeDuration).SetEase(Ease.InQuad)
                .OnComplete(() =>
                {
                    extraPanelContainer.DOSizeDelta(new(0, extraPanelContainer.sizeDelta.y), scaleXChangeDuration).SetEase(Ease.InSine)
                        .OnComplete(() =>
                        {
                            action?.Invoke();
                        });
                });

        isOpened = false;
    }
}
