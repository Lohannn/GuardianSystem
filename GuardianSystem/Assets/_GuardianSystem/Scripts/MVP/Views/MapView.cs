using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MapView : MonoBehaviour
{
    [Header("Crisis Animation Config")]
    [SerializeField] private float startWidth = 0.02f;
    [SerializeField] private float endWidth = 0.5f;
    [SerializeField] private float animationDuration = 1.2f;

    [Header("Map Shake Animation Config")]
    [SerializeField] private float shakeDuration;
    [SerializeField] private float shakeStrength;
    [SerializeField] private int shakeVibrato;

    private Material mapMaterial;
    private Sequence crisisSequence;

    //Shader properties
    private static readonly int CrisisWidth = Shader.PropertyToID("_CrisisWidth");
    private static readonly int CrisisIntensity = Shader.PropertyToID("_CrisisIntensity");

    private static readonly int NorthActive = Shader.PropertyToID("_NorthActive");
    private static readonly int SouthActive = Shader.PropertyToID("_SouthActive");
    private static readonly int CenterActive = Shader.PropertyToID("_CenterActive");
    private static readonly int EastActive = Shader.PropertyToID("_EastActive");
    private static readonly int WestActive= Shader.PropertyToID("_WesthActive");

    private bool isShaking = false;

    private Image mapImage;
    private RectTransform rTransform;

    void Awake()
    {
        mapImage = GetComponent<Image>();
        rTransform = GetComponent<RectTransform>();
        mapMaterial = mapImage.material;

        ResetCrisis();
    }

    void OnEnable()
    {
        EventBus.OnWaterOverflow += AnimateWaterCrisis;
        GameManager.Instance.OnSetupDay += ResetCrisis;
    }

    void OnDisable()
    {
        EventBus.OnWaterOverflow += AnimateWaterCrisis;
        GameManager.Instance.OnSetupDay += ResetCrisis;

        ResetCrisis();
    }

    private void AnimateWaterCrisis(Zone zone)
    {
        mapMaterial.SetFloat(GetZoneProperty(zone), 1);

        crisisSequence?.Kill();

        mapMaterial.SetFloat(CrisisWidth, startWidth);
        mapMaterial.SetFloat(CrisisIntensity, 0.8f);

        ShakeMap();

        crisisSequence = DOTween.Sequence();

        crisisSequence.Join(
            DOTween.To(
                () => mapMaterial.GetFloat(CrisisWidth),
                value => mapMaterial.SetFloat(CrisisWidth, value),
                endWidth,
                animationDuration
            ).SetEase(Ease.OutBack)
        );

        crisisSequence.Join(
            DOTween.To(
                () => mapMaterial.GetFloat(CrisisIntensity),
                value => mapMaterial.SetFloat(CrisisIntensity, value),
                0f,
                animationDuration
            ).SetEase(Ease.InQuad)
        );

        crisisSequence.OnComplete(ResetCrisisValues);
    }

    private void ShakeMap()
    {
        if (isShaking) return;

        isShaking = true;
        rTransform.DOShakeAnchorPos(shakeDuration, shakeStrength, shakeVibrato).OnComplete(() => {
            isShaking = false;
        });
    }

    private void ResetCrisis()
    {
        crisisSequence?.Kill();
        ResetCrisisValues();
    }

    private void ResetCrisisValues()
    {
        if (mapMaterial == null) return;

        mapMaterial.SetFloat(NorthActive, 0f);
        mapMaterial.SetFloat(SouthActive, 0f);
        mapMaterial.SetFloat(EastActive, 0f);
        mapMaterial.SetFloat(WestActive, 0f);
        mapMaterial.SetFloat(CenterActive, 0f);

        mapMaterial.SetFloat(CrisisIntensity, 0f);
        mapMaterial.SetFloat(CrisisWidth, startWidth);
    }

    private int GetZoneProperty(Zone zone)
    {
        return zone switch
        {
            Zone.North => NorthActive,
            Zone.South => SouthActive,
            Zone.Center => CenterActive,
            Zone.West => WestActive,
            Zone.East => EastActive,
            _ => CenterActive,
        };
    }
}
