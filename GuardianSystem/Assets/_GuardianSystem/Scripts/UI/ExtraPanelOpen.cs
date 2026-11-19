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
        extraPanelContainer.DOKill();

        extraPanelContainer.sizeDelta = new(0, 40);
        isOpened = false;
    }

    public void TogglePanelWindowAnimated(Action action)
    {
        extraPanelContainer.DOKill();

        if (isOpened)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.GetSFX(SFXType.UI_Panel_Close));

            extraPanelContainer.DOSizeDelta(new(openedSize.x, 40), scaleYChangeDuration).SetUpdate(true).SetEase(Ease.InQuad)
                .OnComplete(() =>
                {
                    extraPanelContainer.DOSizeDelta(new(0, 40), scaleXChangeDuration).SetUpdate(true).SetEase(Ease.InSine)
                        .OnComplete(() =>
                        {
                            action?.Invoke();
                        });
                });
        }
        else
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.GetSFX(SFXType.UI_Panel_Open));

            extraPanelContainer.DOSizeDelta(new(openedSize.x, 40), scaleXChangeDuration).SetUpdate(true).SetEase(Ease.OutSine)
                .OnComplete(() =>
                {
                    extraPanelContainer.DOSizeDelta(new(openedSize.x, openedSize.y), scaleYChangeDuration).SetUpdate(true).SetEase(Ease.OutBack)
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
        extraPanelContainer.DOKill();

        if (isOpened)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.GetSFX(SFXType.UI_Panel_Close));
            extraPanelContainer.DOSizeDelta(new(openedSize.x, 40), scaleYChangeDuration).SetUpdate(true).SetEase(Ease.InQuad)
                .OnComplete(() =>
                {
                    extraPanelContainer.DOSizeDelta(new(0, 40), scaleXChangeDuration).SetUpdate(true).SetEase(Ease.InSine);
                });
        }
        else
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.GetSFX(SFXType.UI_Panel_Open));
            extraPanelContainer.DOSizeDelta(new(openedSize.x, 40), scaleXChangeDuration).SetUpdate(true).SetEase(Ease.OutSine)
                .OnComplete(() =>
                {
                    extraPanelContainer.DOSizeDelta(new(openedSize.x, openedSize.y), scaleYChangeDuration).SetUpdate(true).SetEase(Ease.OutBack);
                });
        }

        isOpened = !isOpened;
    }

    public void OpenPanelAnimated()
    {
        extraPanelContainer.DOKill();

        AudioManager.Instance.PlaySFX(AudioManager.Instance.GetSFX(SFXType.UI_Panel_Open));
        extraPanelContainer.DOSizeDelta(new(openedSize.x, extraPanelContainer.sizeDelta.y), scaleXChangeDuration).SetUpdate(true).SetEase(Ease.OutSine)
                .OnComplete(() =>
                {
                    extraPanelContainer.DOSizeDelta(new(openedSize.x, openedSize.y), scaleYChangeDuration).SetUpdate(true).SetEase(Ease.OutBack);
                });

        isOpened = true;
    }

    public void OpenPanelAnimated(Action action)
    {
        extraPanelContainer.DOKill();

        AudioManager.Instance.PlaySFX(AudioManager.Instance.GetSFX(SFXType.UI_Panel_Open));
        extraPanelContainer.DOSizeDelta(new(openedSize.x, extraPanelContainer.sizeDelta.y), scaleXChangeDuration).SetUpdate(true).SetEase(Ease.OutSine)
                .OnComplete(() =>
                {
                    extraPanelContainer.DOSizeDelta(new(openedSize.x, openedSize.y), scaleYChangeDuration).SetUpdate(true).SetEase(Ease.OutBack)
                        .OnComplete(() =>
                        {
                            action?.Invoke();
                        });
                });

        isOpened = true;
    }

    public void ClosePanelAnimated()
    {
        extraPanelContainer.DOKill();

        AudioManager.Instance.PlaySFX(AudioManager.Instance.GetSFX(SFXType.UI_Panel_Close));
        extraPanelContainer.DOSizeDelta(new(extraPanelContainer.sizeDelta.x, 40), scaleYChangeDuration).SetUpdate(true).SetEase(Ease.InQuad)
                .OnComplete(() =>
                {
                    extraPanelContainer.DOSizeDelta(new(0, extraPanelContainer.sizeDelta.y), scaleXChangeDuration).SetUpdate(true).SetEase(Ease.InSine);
                });

        isOpened = false;
    }

    public void ClosePanelAnimated(Action action)
    {
        extraPanelContainer.DOKill();

        AudioManager.Instance.PlaySFX(AudioManager.Instance.GetSFX(SFXType.UI_Panel_Close));
        extraPanelContainer.DOSizeDelta(new(extraPanelContainer.sizeDelta.x, 40), scaleYChangeDuration).SetUpdate(true).SetEase(Ease.InQuad)
                .OnComplete(() =>
                {
                    extraPanelContainer.DOSizeDelta(new(0, extraPanelContainer.sizeDelta.y), scaleXChangeDuration).SetUpdate(true).SetEase(Ease.InSine)
                        .OnComplete(() =>
                        {
                            action?.Invoke();
                        });
                });

        isOpened = false;
    }
}
