using DG.Tweening;
using System.Collections;
using UnityEngine;

public class UnitBase : MonoBehaviour
{
    [Header("Animation Config")]
    [SerializeField] private float floatHeight;
    [SerializeField] private float cycleDuration;

    private RectTransform rTransform;

    private void Awake()
    {
        rTransform = GetComponent<RectTransform>();

        StartCoroutine(RandomStartTime());
    }

    private IEnumerator RandomStartTime()
    {
        float randomStart = Random.Range(0, 0.81f);

        yield return new WaitForSeconds(randomStart);
        rTransform.DOAnchorPosY(rTransform.anchoredPosition.y + floatHeight, cycleDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }
}
