using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPanelView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<GameObject> tutorialPages;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;

    private int currentPageIndex = 0;

    private void Awake()
    {
        nextButton.onClick.AddListener(NextPage);
        prevButton.onClick.AddListener(PreviousPage);
    }

    private void OnEnable()
    {
        ShowPage(0);
    }

    public void NextPage()
    {
        ShowPage(currentPageIndex + 1);
    }

    public void PreviousPage()
    {
        ShowPage(currentPageIndex - 1);
    }

    private void ShowPage(int index)
    {
        if (tutorialPages.Count == 0)
        {
            nextButton.gameObject.SetActive(false);
            prevButton.gameObject.SetActive(false);
            return;
        }

        if (index < 0 || index >= tutorialPages.Count)
            return;

        currentPageIndex = index;

        for (int i = 0; i < tutorialPages.Count; i++)
        {
            tutorialPages[i].SetActive(i == currentPageIndex);
        }

        UpdateButtons();
    }

    private void UpdateButtons()
    {
        prevButton.gameObject.SetActive(currentPageIndex > 0);
        nextButton.gameObject.SetActive(currentPageIndex < tutorialPages.Count - 1);
    }

    private void OnDestroy()
    {
        nextButton.onClick.RemoveListener(PreviousPage);
        prevButton.onClick.RemoveListener(NextPage);
    }
}
