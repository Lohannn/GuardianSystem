using UnityEngine;
using UnityEngine.UI;

public class MainMenuView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform menuBackground;
    [SerializeField] private RectTransform gameMenuBackground;
    [SerializeField] private Image logo;
    [SerializeField] private ExtraPanelOpen playPanel;
    [SerializeField] private Image mapImage;
    [SerializeField] private CanvasGroup buttonsContainerGroup;
}
