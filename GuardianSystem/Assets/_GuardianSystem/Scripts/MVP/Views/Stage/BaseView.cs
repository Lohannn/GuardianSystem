using DG.Tweening;
using System.Collections;
using UnityEngine;

public class BaseView : MonoBehaviour
{
    [Header("Animation Config")]
    [SerializeField] private float animDuration = 0.2f;

    private RectTransform rTransform;

    private void Awake()
    {
        rTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        StartCoroutine(AnimationDelay());
    }

    private IEnumerator AnimationDelay()
    {
        yield return new WaitForSeconds(0.3f);
        rTransform.DOSizeDelta(new(rTransform.sizeDelta.x, 50f), animDuration).SetEase(Ease.OutBack);
    }
}
