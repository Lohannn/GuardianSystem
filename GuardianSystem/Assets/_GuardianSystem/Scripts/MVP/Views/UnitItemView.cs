using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitItemView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private UnitItemPresenter presenter;
    [SerializeField] private Image dispatchStatusIcon;
    [SerializeField] private Image dispatchCooldownImage;
    [SerializeField] private CanvasGroup parentContainer;

    private float maxTime;
    private float remainingTime;

    void OnEnable()
    {
        GameManager.Instance.OnDayEnded += EndDispatch;
    }

    void OnDisable()
    {
        GameManager.Instance.OnDayEnded -= EndDispatch;
    }

    void Start()
    {
        if (presenter == null)
            presenter = GetComponent<UnitItemPresenter>();
    }

    public void StartDispatch(float dispatchTime)
    {
        dispatchCooldownImage.gameObject.SetActive(true);
        dispatchStatusIcon.gameObject.SetActive(true);
        maxTime = dispatchTime;
        remainingTime = maxTime;
    }

    private void EndDispatch()
    {
        dispatchCooldownImage.fillAmount = 1;
        dispatchCooldownImage.gameObject.SetActive(false);
        dispatchStatusIcon.gameObject.SetActive(false);

        presenter.EndDispatchMode();
    }

    void Update()
    {
        if (GameManager.Instance.CurrentGameState == GameState.Running || 
            GameManager.Instance.CurrentGameState == GameState.Utopia ||
            GameManager.Instance.CurrentGameState == GameState.Overtime)
        {
            if (remainingTime > 0)
            {
                remainingTime -= Time.deltaTime;
                dispatchCooldownImage.fillAmount = Mathf.Clamp01(remainingTime / maxTime);

                if (remainingTime <= 0)
                {
                    EndDispatch();
                }
            }
        }
    }

    public void BeginDrag()
    {
        parentContainer.alpha = 0.3f;
        parentContainer.interactable = false;
        parentContainer.blocksRaycasts = false;
    }

    public void EndDrag()
    {
        parentContainer.alpha = 1f;
        parentContainer.interactable = true;
        parentContainer.blocksRaycasts = true;
    }
}
