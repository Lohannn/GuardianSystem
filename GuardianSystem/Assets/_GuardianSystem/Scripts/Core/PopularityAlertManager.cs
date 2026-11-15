using System.Collections.Generic;
using UnityEngine;

public class PopularityAlertManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ObjectPool alertPool;

    [Header("Alert position config")]
    [SerializeField] private float activeAlertAnchoredPosition;

    private List<PopularityAlertView> alertList = new();

    void OnEnable()
    {
        EventBus.OnPopularityValueChanged += SpawnAlert;
        EventBus.OnAlertExpire += RemoveAlert;
    }

    void OnDisable()
    {
        EventBus.OnPopularityValueChanged -= SpawnAlert;
        EventBus.OnAlertExpire -= RemoveAlert;
    }

    private void SpawnAlert(bool isPositive)
    {
        GameObject alert = alertPool.GetObjectFromPool();
        PopularityAlertView alertView = alert.GetComponent<PopularityAlertView>();

        alert.SetActive(true);

        alertView.ObjectTransform.SetParent(transform, false);
        alertView.ObjectTransform.anchoredPosition = Vector2.zero;

        alertView.ConfigureAlert(isPositive);

        alertList.Insert(0, alertView);

        alertView.AnimateEntry(activeAlertAnchoredPosition);
        UpdatePositions();
    }

    private void RemoveAlert(PopularityAlertView alert)
    {
        if (!alertList.Remove(alert)) return;

        alertPool.ReturnObjectToPool(alert.gameObject);
        UpdatePositions();
    }

    private void UpdatePositions()
    {
        for (int i = 0; i < alertList.Count; i++)
        {
            PopularityAlertView alert = alertList[i];

            alert.MoveTo(-i * (alert.ObjectTransform.sizeDelta.y + 50f));
        }
    }
}
