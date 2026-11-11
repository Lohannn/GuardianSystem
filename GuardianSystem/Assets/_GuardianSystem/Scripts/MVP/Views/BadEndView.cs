using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BadEndView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image badEndImage;
    [SerializeField] private CanvasGroup badEndMessageGroup;
    [SerializeField] private TextMeshProUGUI firstText;
    [SerializeField] private TextMeshProUGUI secondText;
    [SerializeField] private RectTransform creditsViewport;
    [SerializeField] private RectTransform creditsContainer;
    private CanvasGroup badEndGroup;

    [Header("Sequence Config")]
    [SerializeField] private float messageTypingSpeedPerCharacter;
    [SerializeField] private float delayBetweenTexts;
    [SerializeField] private float messageFadeDuration;
    [SerializeField] private float creditsRollSpeed;

    private float typingTimer;
    private bool canFirstTextStartTyping;
    private bool canSecondTextStartTyping;
    private bool canCreditsRoll;

    private void Awake()
    {
        badEndGroup = GetComponent<CanvasGroup>();

        firstText.ForceMeshUpdate();
        secondText.ForceMeshUpdate();

        firstText.maxVisibleCharacters = 0;
        secondText.maxVisibleCharacters = 0;
    }

    private void OnEnable()
    {
        GameManager.Instance.OnDayInterrupted += StartEnd;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnDayInterrupted -= StartEnd;
    }

    private void Update()
    {
        if (canFirstTextStartTyping || canSecondTextStartTyping)
        {
            typingTimer += Time.deltaTime;
            int visibleCharacters = Mathf.FloorToInt(typingTimer / messageTypingSpeedPerCharacter);

            if (canFirstTextStartTyping)
            {
                firstText.maxVisibleCharacters = visibleCharacters;
            }
            else if (canSecondTextStartTyping)
            {
                secondText.maxVisibleCharacters = visibleCharacters;
            }
        }

        
    }

    private void StartEnd()
    {
        badEndGroup.alpha = 1;
        badEndGroup.interactable = true;
        badEndGroup.blocksRaycasts = true;
        badEndImage.gameObject.SetActive(true);
        StartCoroutine(EndingCoroutine());
    }

    private void RollCredits()
    {
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(creditsContainer);

        float contentHeight = creditsContainer.rect.height;
        float viewportHeight = creditsViewport.rect.height;

        float startY = -(viewportHeight + contentHeight) / 2;
        float endY = (viewportHeight + contentHeight) / 2;

        float distance = endY - startY;
        float duration = distance / creditsRollSpeed;

        creditsContainer.anchoredPosition = new(creditsContainer.anchoredPosition.x, startY);

        creditsContainer.DOAnchorPosY(endY, duration).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                StartCoroutine(GoToMainMenu());
            });
    }

    private IEnumerator EndingCoroutine()
    {
        yield return new WaitForSeconds(2.5f);

        canFirstTextStartTyping = true;

        while (firstText.maxVisibleCharacters < firstText.textInfo.characterCount)
        {
            yield return null;
        }

        typingTimer = 0;
        canFirstTextStartTyping = false;

        yield return new WaitForSeconds(delayBetweenTexts);

        canSecondTextStartTyping = true;

        while (secondText.maxVisibleCharacters < secondText.textInfo.characterCount)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        badEndMessageGroup.DOFade(0, messageFadeDuration);

        RollCredits();
    }

    private IEnumerator GoToMainMenu()
    {
        yield return new WaitForSeconds(1.5f);

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
