using UnityEngine;

public class ZoneView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ZonePresenter presenter;
    [SerializeField] private GameObject[] trashIcon;

    public void UpdateIcons(int currentCounters)
    {
        for (int i = 0; i < trashIcon.Length; i++)
        {
            trashIcon[i].SetActive(i < currentCounters);
        }
    }
}
