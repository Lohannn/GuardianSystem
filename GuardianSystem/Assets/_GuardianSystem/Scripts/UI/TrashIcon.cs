using DG.Tweening;
using UnityEngine;

public class TrashIcon : MonoBehaviour
{
    [Header("Animation Config")]
    [SerializeField] private Vector2 finalSize;
    [SerializeField] private float animationDuration;

    private RectTransform rTransform;

    void Awake()
    {
        rTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        rTransform.DOSizeDelta(finalSize, animationDuration).SetEase(Ease.OutBack);
    }

    private void OnDisable()
    {
        rTransform.sizeDelta = Vector2.zero;
    }
}
