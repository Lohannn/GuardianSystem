using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum TextBoxState
{
    Typing,
    Finished
}

public class CutsceneView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CutscenePresenter presenter;
    [SerializeField] private CanvasGroup cutsceneContainer;
    [SerializeField] private Image cutsceneBackground;
    [SerializeField] private CanvasGroup textBox;
    [SerializeField] private GameObject passTextIcon;
    [SerializeField] private TextMeshProUGUI dialogText;

    [SerializeField] private Button toggleVisibilityButton;
    [SerializeField] private Sprite visibleIcon;
    [SerializeField] private Sprite invisibleIcon;

    [SerializeField] private float secondsPerCharacter = 0.03f;
    private float typingTimer;

    private bool isVisible;
    private bool canToggleVisibility = true;
    private TextBoxState currentState = TextBoxState.Finished;

    private string[] currentDialogs;
    private int currentDialogIndex;
    private string currentText;

    void Awake()
    {
        toggleVisibilityButton.onClick.AddListener(ToggleDialog);
    }

    void Update()
    {
        if (currentState != TextBoxState.Typing) return;

        typingTimer += Time.deltaTime;

        int visibleCharacters = Mathf.FloorToInt(typingTimer / secondsPerCharacter);

        dialogText.maxVisibleCharacters = visibleCharacters;

        if (visibleCharacters >= dialogText.textInfo.characterCount)
            ChangeState(TextBoxState.Finished);
    }

    public void ChangeState(TextBoxState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case TextBoxState.Typing:
                StartTyping();
                break;
            case TextBoxState.Finished:
                FinishCurrentDialog();
                break;
        }
    }

    public void SetupCutscene(CutsceneData cutscene, string[] dialogs)
    {
        currentDialogs = dialogs;
        currentDialogIndex = 0;
        currentText = currentDialogs[currentDialogIndex];
        cutsceneBackground.sprite = cutscene.cutsceneImage;
        cutsceneContainer.alpha = 1f;
        cutsceneContainer.blocksRaycasts = true;
        cutsceneContainer.interactable = true;

        StartCoroutine(StartCutsceneCourotine());
    }

    public void StartCutscene()
    {
        dialogText.maxVisibleCharacters = 0;

        canToggleVisibility = true;
        ToggleDialog();

        ChangeState(TextBoxState.Typing);
    }

    private void StartTyping()
    {
        dialogText.text = currentText;
        dialogText.ForceMeshUpdate();

        dialogText.maxVisibleCharacters = 0;

        typingTimer = 0f;
        passTextIcon.SetActive(false);
    }

    private void FinishCurrentDialog()
    {
        dialogText.maxVisibleCharacters = dialogText.textInfo.characterCount;
        passTextIcon.SetActive(true);
    }

    public void EndCutscene()
    {
        dialogText.text = "";
        currentDialogIndex = 0;
        currentDialogs = null;
        currentText = "";
        isVisible = false;

        cutsceneContainer.alpha = 0f;
        cutsceneContainer.blocksRaycasts = false;
        cutsceneContainer.interactable = false;

        presenter.EndCutscene();
    }

    public void Interact()
    {
        if (currentState == TextBoxState.Typing)
        {
            ChangeState(TextBoxState.Finished);
        }else
        {
            currentDialogIndex++;

            if (currentDialogIndex >= currentDialogs.Length)
            {
                ToggleDialog();
                canToggleVisibility = false;
                StartCoroutine(EndCutsceneCoroutine());
                return;
            }

            currentText = currentDialogs[currentDialogIndex];
            ChangeState(TextBoxState.Typing);
        }
    }

    public void ToggleDialog()
    {
        if (!canToggleVisibility) return;

        if (isVisible)
        {
            textBox.alpha = 0f;
            isVisible = false;
            textBox.interactable = false;
            textBox.blocksRaycasts = false;
            isVisible = false;
            toggleVisibilityButton.image.sprite = invisibleIcon;
        }else
        {
            textBox.alpha = 1f;
            isVisible = true;
            textBox.interactable = true;
            textBox.blocksRaycasts = true;
            isVisible = true;
            toggleVisibilityButton.image.sprite = visibleIcon;
        }
    }

    private IEnumerator StartCutsceneCourotine()
    {
        yield return new WaitForSeconds(2f);
        StartCutscene();
    }

    private IEnumerator EndCutsceneCoroutine()
    {
        yield return new WaitForSeconds(2f);
        EndCutscene();
    }
}
