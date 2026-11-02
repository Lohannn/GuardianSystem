using UnityEngine;

public class UnitInfoOpen : MonoBehaviour
{
    [SerializeField] private RectTransform unitInfoContainer;
    private bool isOpened = false;

    public void ToggleUnitsInfo()
    {
        unitInfoContainer.sizeDelta = new(unitInfoContainer.sizeDelta.x, isOpened ? 0f : 200f);
        isOpened = !isOpened;
    }
}
