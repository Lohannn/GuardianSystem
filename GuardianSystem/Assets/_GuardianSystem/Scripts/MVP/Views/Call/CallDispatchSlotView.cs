using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CallDispatchSlotView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CallDispatchSlotPresenter presenter;
    [SerializeField] private Image unitIcon;

    void OnDisable()
    {
        RemoveIcon();
    }

    public void SetIcon(UnitSO unit)
    {
        unitIcon.sprite = unit.icon;
        unitIcon.color = new(unitIcon.color.r, unitIcon.color.g, unitIcon.color.b, 1f);
    }

    public void RemoveIcon()
    {
        unitIcon.sprite = null;
        unitIcon.color = new(unitIcon.color.r, unitIcon.color.g, unitIcon.color.b, 0f);
    }
}
