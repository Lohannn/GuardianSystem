using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GhostItem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject previewContainer;
    [SerializeField] private TextMeshProUGUI dispatchDurationPreview;
    
    private Image unitImage;
    private RectTransform rTransform;

    void OnEnable()
    {
        EventBus.OnUnitBeginDrag += StartGhost;
        EventBus.OnUnitDrag += UpdateGhost;
        EventBus.OnUnitEndDrag += EndGhost;
        EventBus.OnShowDispatchPreview += ShowDurationPreview;
        EventBus.OnHideDispatchPreview += HideDurationPreview;
    }

    void OnDisable()
    {
        EventBus.OnUnitBeginDrag -= StartGhost;
        EventBus.OnUnitDrag -= UpdateGhost;
        EventBus.OnUnitEndDrag -= EndGhost;
        EventBus.OnShowDispatchPreview -= ShowDurationPreview;
        EventBus.OnHideDispatchPreview -= HideDurationPreview;
    }

    void Start()
    {
        rTransform = GetComponent<RectTransform>();
        unitImage = GetComponent<Image>();
        unitImage.color = new Color(unitImage.color.r, unitImage.color.g, unitImage.color.b, 0f);
    }

    private void ShowDurationPreview(float duration)
    {
        dispatchDurationPreview.SetText("{0:0.0} seg", duration);
        previewContainer.SetActive(true);
    }

    private void HideDurationPreview()
    {
        previewContainer.SetActive(false);
    }

    private void StartGhost(UnitItemPresenter presenter, Vector2 position)
    {
        unitImage.sprite = presenter.Unit.icon;
        unitImage.color = new Color(unitImage.color.r, unitImage.color.g, unitImage.color.b, 0.5f);
        rTransform.position = position;
    }   

    public void UpdateGhost(Vector2 position)
    {
        rTransform.position = position;
    }

    public void EndGhost()
    {
        unitImage.sprite = null;
        unitImage.color = new Color(unitImage.color.r, unitImage.color.g, unitImage.color.b, 0f);
        HideDurationPreview();
    }
}
