using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform menuBackground;
    [SerializeField] private Image menuBackgroundImage;
    [SerializeField] private RectTransform gameMenuBackground;
    [SerializeField] private Image logo;
    [SerializeField] private ExtraPanelOpen playPanel;
    [SerializeField] private Image mapImage;
    [SerializeField] private RectTransform mapImageTransform;
    [SerializeField] private CanvasGroup buttonsContainerGroup;
    [SerializeField] private CanvasGroup gameButtonsContainerGroup;

    private Sequence transitionSequence;

    public void StartGameAnimation()
    {
        if (transitionSequence != null && transitionSequence.IsActive())
            return;

        playPanel.ClosePanelAnimated();

        buttonsContainerGroup.interactable = false;
        buttonsContainerGroup.blocksRaycasts = false;

        gameButtonsContainerGroup.alpha = 0f;
        gameButtonsContainerGroup.interactable = false;
        gameButtonsContainerGroup.blocksRaycasts = false;

        Vector2 finalSize = new Vector2(326.3461f, 1125.015f);
        Vector2 finalMapPosition = new Vector2(184f, -40f);

        transitionSequence = DOTween.Sequence();

        transitionSequence.Insert(0f,
            buttonsContainerGroup.DOFade(0f, 0.3f)
                .SetEase(Ease.OutQuad));

        transitionSequence.Insert(0.05f,
            logo.rectTransform.DOScale(
                logo.rectTransform.localScale * 1.08f, 0.5f)
                .SetEase(Ease.OutCubic));

        transitionSequence.Insert(0.1f,
            logo.DOFade(0f, 0.45f));

        transitionSequence.Insert(0.2f,
            mapImage.DOFade(1f, 0.9f));

        transitionSequence.Insert(0.2f,
            mapImageTransform.DOAnchorPos(finalMapPosition, 1.1f)
                .SetEase(Ease.InOutCubic));

        transitionSequence.Insert(0.25f,
            menuBackgroundImage.DOFade(0f, 0.85f));

        transitionSequence.Insert(0.25f,
            menuBackground.DOSizeDelta(finalSize, 1.1f)
                .SetEase(Ease.InOutCubic));

        transitionSequence.Insert(0.32f,
            gameMenuBackground.DOSizeDelta(finalSize, 1.1f)
                .SetEase(Ease.InOutCubic));

        transitionSequence.Insert(1.25f,
            gameButtonsContainerGroup.DOFade(1f, 0.5f)
                .SetEase(Ease.OutQuad));

        transitionSequence.AppendInterval(0.3f);

        transitionSequence.OnComplete(() =>
        {
            SceneManager.LoadScene("StageScene");
        });
    }

    private void OnDestroy()
    {
        transitionSequence?.Kill();
    }
}