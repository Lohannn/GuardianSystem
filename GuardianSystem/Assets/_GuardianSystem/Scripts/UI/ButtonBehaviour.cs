using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Button Components")]
    [SerializeField] private Button button;
    [SerializeField] private RectTransform buttonTransform;
    [SerializeField] private Image buttonOffSprite;
    [SerializeField] private RectTransform outerDetailsContainer;
    [SerializeField] private CanvasGroup outerDetailsContainerGroup;
    [SerializeField] private RectTransform innerDetailsContainer;
    [SerializeField] private CanvasGroup innerDetailsContainerGroup;

    [Header("Animation Config")]
    [SerializeField] private float scaleChangeDuration;
    [SerializeField] private float hoverScaleMultiplier;
    [SerializeField] private float pressedScaleMultiplier;
    private Vector3 defaultScale;
    [SerializeField] private float innerDetailsDelay;
    [SerializeField] private float detailsOpacityChangeDuration;
    [SerializeField] private float detailsMovementDuration;
    [SerializeField] private float outerDetailsScaleMultiplierX;
    [SerializeField] private float outerDetailsScaleMultiplierY;
    [SerializeField] private float innerDetailsScaleMultiplierX;
    [SerializeField] private float innerDetailsScaleMultiplierY;
    private float outerDetailDefaultScaleX;
    private float outerDetailDefaultScaleY;
    private float innerDetailDefaultScaleX;
    private float innerDetailDefaultScaleY;
    [SerializeField] private float colorChangeDuration;

    private bool isPressed = false;
    private bool isReleasePending = false;

    private bool lastInteractableState = true;

    void Awake()
    {
        defaultScale = buttonTransform.localScale;
        outerDetailDefaultScaleX = outerDetailsContainer.localScale.x;
        outerDetailDefaultScaleY = outerDetailsContainer.localScale.y;
        innerDetailDefaultScaleX = innerDetailsContainer.localScale.x;
        innerDetailDefaultScaleY = innerDetailsContainer.localScale.y;
    }

    void LateUpdate()
    {
        if (isReleasePending)
        {
            isReleasePending = false;

            if (button.interactable) RealeaseButton();
        }

        if (button.interactable != lastInteractableState)
        {
            lastInteractableState = button.interactable;

            if (button.interactable)
                ResetVisual();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isReleasePending = false;

        buttonTransform.DOScale((defaultScale * pressedScaleMultiplier), scaleChangeDuration);
        buttonOffSprite.DOFade(0, colorChangeDuration);
        outerDetailsContainerGroup.DOFade(1, detailsOpacityChangeDuration);
        outerDetailsContainer.DOScaleX((outerDetailDefaultScaleX * outerDetailsScaleMultiplierX), detailsMovementDuration);
        outerDetailsContainer.DOScaleY((outerDetailDefaultScaleY * outerDetailsScaleMultiplierY), detailsMovementDuration);
        innerDetailsContainerGroup.DOFade(1, detailsOpacityChangeDuration).SetDelay(innerDetailsDelay);
        innerDetailsContainer.DOScaleX((innerDetailDefaultScaleX * innerDetailsScaleMultiplierX), detailsMovementDuration).SetDelay(innerDetailsDelay);
        innerDetailsContainer.DOScaleY((innerDetailDefaultScaleY * innerDetailsScaleMultiplierY), detailsMovementDuration).SetDelay(innerDetailsDelay);

        isPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isReleasePending = true;
    }

    private void RealeaseButton()
    {
        buttonTransform.DOScale((defaultScale), scaleChangeDuration);
        buttonOffSprite.DOFade(1, colorChangeDuration);
        outerDetailsContainerGroup.DOFade(0, detailsOpacityChangeDuration);
        outerDetailsContainer.DOScaleX((outerDetailDefaultScaleX), detailsMovementDuration);
        outerDetailsContainer.DOScaleY((outerDetailDefaultScaleY), detailsMovementDuration);
        innerDetailsContainerGroup.DOFade(0, detailsOpacityChangeDuration).SetDelay(innerDetailsDelay);
        innerDetailsContainer.DOScaleX((innerDetailDefaultScaleX), detailsMovementDuration).SetDelay(innerDetailsDelay).SetEase(Ease.InQuad);
        innerDetailsContainer.DOScaleY((innerDetailDefaultScaleY), detailsMovementDuration).SetDelay(innerDetailsDelay).SetEase(Ease.InQuad);

        isPressed = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isPressed || !button.interactable) return;

        buttonTransform.DOScale((defaultScale * hoverScaleMultiplier), scaleChangeDuration);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isPressed || !button.interactable) return;

        buttonTransform.DOScale((defaultScale), scaleChangeDuration);
    }

    private void ResetVisual()
    {
        buttonTransform.DOScale((defaultScale), scaleChangeDuration);
        buttonOffSprite.DOFade(1, colorChangeDuration);
        outerDetailsContainerGroup.DOFade(0, detailsOpacityChangeDuration);
        outerDetailsContainer.DOScaleX((outerDetailDefaultScaleX), detailsMovementDuration);
        outerDetailsContainer.DOScaleY((outerDetailDefaultScaleY), detailsMovementDuration);
        innerDetailsContainerGroup.DOFade(0, detailsOpacityChangeDuration).SetDelay(innerDetailsDelay);
        innerDetailsContainer.DOScaleX((innerDetailDefaultScaleX), detailsMovementDuration).SetDelay(innerDetailsDelay).SetEase(Ease.InQuad);
        innerDetailsContainer.DOScaleY((innerDetailDefaultScaleY), detailsMovementDuration).SetDelay(innerDetailsDelay).SetEase(Ease.InQuad);

        isPressed = false;
    }
}
