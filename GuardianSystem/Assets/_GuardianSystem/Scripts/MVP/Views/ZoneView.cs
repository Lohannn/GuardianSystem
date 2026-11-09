using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ZoneView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ZonePresenter presenter;
    [SerializeField] private GameObject[] trashIcon;
    [SerializeField] private RectTransform statsIcon;

    public void UpdateIcons(int currentCounters)
    {
        for (int i = 0; i < trashIcon.Length; i++)
        {
            trashIcon[i].SetActive(i < currentCounters);
        }
    }

    public void ActivateStatus()
    {
        statsIcon.gameObject.SetActive(true);

        statsIcon.DORotate(new(0, 0, 180), 0.6f).SetLoops(-1, LoopType.Incremental);
    }

    public void DeactivateStatus()
    {
        statsIcon.DOKill();
        statsIcon.gameObject.SetActive(false);
    }
}
