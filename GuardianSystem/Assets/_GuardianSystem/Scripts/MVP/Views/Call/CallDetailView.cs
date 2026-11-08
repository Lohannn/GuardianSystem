using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CallDetailView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CallDetailPresenter presenter;
    [SerializeField] private ExtraPanelOpen animation;

    [SerializeField] private Image clickBlockerPanel;
    [SerializeField] private Image callDetailPanel;
    [SerializeField] private Image callSituationImage;
    [SerializeField] private TextMeshProUGUI callDescriptionText;
    [SerializeField] private Button[] questionCallAnswerButtons;
    [SerializeField] private Button confirmAnswerButton;

    [SerializeField] private RectTransform twoAnswersContainer;
    [SerializeField] private RectTransform threeAnswersUpperContainer;
    [SerializeField] private RectTransform threeAnswersLowerContainer;
    [SerializeField] private RectTransform fourAnswersContainer;
    [SerializeField] private Image[] dispatchSlots;
    [SerializeField] private RectTransform dispatchContainer;

    [Header("Event Data")]
    [SerializeField] private EventData data;

    private List<Button> alreadySetupedButtons;

    private void Awake()
    {
        confirmAnswerButton.onClick.RemoveAllListeners();
        confirmAnswerButton.onClick.AddListener(OnAnswerConfirm);
    }

    private void OnEnable()
    {
        EventBus.OnCallClicked += HandleCallClicked;
    }

    private void OnDisable()
    {
        EventBus.OnCallClicked -= HandleCallClicked;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) && callDetailPanel.gameObject.activeInHierarchy)
        {
            Hide();
            EventBus.OnDetailPanelHide.Invoke();
        }
    }

    private void Initialize(EventData eventData)
    {
        data = eventData;

        callDescriptionText.text = data.eventDescription;
        callSituationImage.sprite = data.eventImage;

        if (data.eventType == EventType.Question)
        {
            alreadySetupedButtons = new List<Button>();
            foreach (EventAnswer answer in data.possibleAnswers)
            {
                int randomIndex = UnityEngine.Random.Range(0, questionCallAnswerButtons.Length);

                if (alreadySetupedButtons.Count < questionCallAnswerButtons.Length)
                {
                    while (alreadySetupedButtons.Contains(questionCallAnswerButtons[randomIndex]))
                    {
                        randomIndex = UnityEngine.Random.Range(0, questionCallAnswerButtons.Length);
                    }
                }

                TextMeshProUGUI buttonText = questionCallAnswerButtons[randomIndex].GetComponentInChildren<TextMeshProUGUI>();
                buttonText.text = answer.answerText;
                questionCallAnswerButtons[randomIndex].onClick.RemoveAllListeners();
                questionCallAnswerButtons[randomIndex].onClick.AddListener(() => OnAnswerSelected(answer, questionCallAnswerButtons[randomIndex]));
                questionCallAnswerButtons[randomIndex].gameObject.SetActive(true);
                alreadySetupedButtons.Add(questionCallAnswerButtons[randomIndex]);
            }

            PositionAnswerButtons(alreadySetupedButtons);
        }
        else
        {
            for (int i = 0; i < data.requiredUnits.Length; i++)
            {
                if (i <= dispatchSlots.Length)
                {
                    dispatchSlots[i].gameObject.SetActive(true);
                    CallDispatchSlotPresenter slot = dispatchSlots[i].GetComponent<CallDispatchSlotPresenter>();
                    slot.callDetail = presenter;
                }
                else
                {
                    Debug.LogWarning($"Not enough dispatch slots for the required units. Required units: {data.requiredUnits.Length}, available slots: {dispatchSlots.Length}");
                }
            }

            dispatchContainer.gameObject.SetActive(true);
        }
    }

    private void PositionAnswerButtons(List<Button> buttons)
    {
        switch (buttons.Count)
        {
            case 2:
                twoAnswersContainer.gameObject.SetActive(true);
                
                foreach (Button button in buttons)
                {
                    button.transform.SetParent(twoAnswersContainer, false);
                }

                threeAnswersUpperContainer.gameObject.SetActive(false);
                threeAnswersLowerContainer.gameObject.SetActive(false);
                fourAnswersContainer.gameObject.SetActive(false);
                break;
            case 3:
                twoAnswersContainer.gameObject.SetActive(false);
                
                threeAnswersUpperContainer.gameObject.SetActive(true);
                threeAnswersLowerContainer.gameObject.SetActive(true);

                foreach (Button button in buttons)
                {
                    if (buttons.IndexOf(button) < 2)
                    {
                        button.transform.SetParent(threeAnswersUpperContainer, false);
                    }
                    else
                    {
                        button.transform.SetParent(threeAnswersLowerContainer, false);
                    }
                }

                fourAnswersContainer.gameObject.SetActive(false);
                break;
            case 4:
                twoAnswersContainer.gameObject.SetActive(false);
                threeAnswersUpperContainer.gameObject.SetActive(false);
                threeAnswersLowerContainer.gameObject.SetActive(false);

                fourAnswersContainer.gameObject.SetActive(true);
                foreach (Button button in buttons)
                {
                    button.transform.SetParent(fourAnswersContainer, false);
                }
                break;
        }
    }

    public void Show(EventData eventData)
    {
        Initialize(eventData);
        
        clickBlockerPanel.color = new Color(clickBlockerPanel.color.r, clickBlockerPanel.color.g, clickBlockerPanel.color.b, 0.5f);
        clickBlockerPanel.raycastTarget = true;

        callDetailPanel.gameObject.SetActive(true);
        animation.OpenPanelAnimated();
    }

    public void Hide()
    {

        foreach (Button button in questionCallAnswerButtons)
        {
            button.onClick.RemoveAllListeners();
            button.interactable = true;
            button.gameObject.SetActive(false);
        }

        alreadySetupedButtons?.Clear();

        foreach (Image slot in dispatchSlots)
        {
            slot.gameObject.SetActive(false);
        }

        twoAnswersContainer.gameObject.SetActive(false);
        threeAnswersUpperContainer.gameObject.SetActive(false);
        threeAnswersLowerContainer.gameObject.SetActive(false);
        fourAnswersContainer.gameObject.SetActive(false);

        clickBlockerPanel.color = new Color(clickBlockerPanel.color.r, clickBlockerPanel.color.g, clickBlockerPanel.color.b, 0f);
        clickBlockerPanel.raycastTarget = false;
        animation.ClosePanelAnimated(() => callDetailPanel.gameObject.SetActive(false));
    }

    private void HandleCallClicked(EventData eventData, CallButtonPresenter callPresenter)
    {
        Show(eventData);
    }

    private void OnAnswerConfirm()
    {
        presenter.CallResolvedHandler(data);
    }

    private void OnAnswerSelected(EventAnswer selectedAnswer, Button selectedButton)
    {
        presenter.HandleAnswerSelection(selectedAnswer);

        selectedButton.interactable = false;

        foreach (Button button in alreadySetupedButtons)
        {
            if (button != selectedButton)
            {
                button.interactable = true;
            }
        }
    }
}
