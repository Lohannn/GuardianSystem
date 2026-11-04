using UnityEngine;

public class ExtraPanelOpen : MonoBehaviour
{
    [SerializeField] private RectTransform extraPanelContainer;
    [SerializeField] private float openedHeight;
    private bool isOpened = false;

    public void TogglePanel()
    {
        extraPanelContainer.sizeDelta = new(extraPanelContainer.sizeDelta.x, isOpened ? 0f : openedHeight);
        isOpened = !isOpened;
    }
}
