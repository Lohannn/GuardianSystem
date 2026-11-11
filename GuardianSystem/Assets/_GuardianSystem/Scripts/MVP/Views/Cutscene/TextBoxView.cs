using UnityEngine;
using UnityEngine.EventSystems;

public class TextBoxView : MonoBehaviour, IPointerClickHandler
{
    [Header("References")]
    [SerializeField] private CutsceneView cutsceneView;

    public void OnPointerClick(PointerEventData eventData)
    {
        cutsceneView.Interact();
    }
}
