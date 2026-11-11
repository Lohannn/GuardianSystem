using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaterLevelButtonView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image buttonOnImage;
    [SerializeField] private Sprite buttonOnCrisisSprite;
    private Sprite defaultButtonOnSprite;
    [SerializeField] private Image buttonOffImage;
    [SerializeField] private Sprite buttonOffCrisisSprite;
    private Sprite defaultButtonOffSprite;

    [SerializeField] private Image[] detailsImages;
    [SerializeField] private Sprite[] crisisDetailsSprites;
    [SerializeField] private Image[] defaultDetailsSprite;

    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private Image waterDrop;
    [SerializeField] private Color32 crisisColor;
    private Color32 defaultColor;

    private bool inCrisis;

    private void OnEnable()
    {
        GameManager.Instance.OnSetupDay += RemoveWaterCrisis;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnSetupDay -= RemoveWaterCrisis;
    }

    private void Start()
    {
        defaultButtonOnSprite = buttonOnImage.sprite;
        defaultButtonOffSprite = buttonOffImage.sprite;
        defaultColor = waterDrop.color;
    }

    public void SetWaterCrisis()
    {
        if (inCrisis) return;

        buttonOnImage.sprite = buttonOnCrisisSprite;
        buttonOffImage.sprite = buttonOffCrisisSprite;
        waterDrop.color = crisisColor;
        label.color = crisisColor;

        for (int i = 0; i < detailsImages.Length; i++)
        {
            detailsImages[i].sprite = crisisDetailsSprites[i];
        }

        inCrisis = true;
    }

    public void RemoveWaterCrisis()
    {
        if (!inCrisis) return;

        buttonOnImage.sprite = defaultButtonOnSprite;
        buttonOffImage.sprite = defaultButtonOffSprite;
        waterDrop.color = defaultColor;
        label.color = defaultColor;

        for (int i = 0; i < detailsImages.Length; i++)
        {
            detailsImages[i].sprite = defaultDetailsSprite[i].sprite;
        }

        inCrisis = false;
    }
}
