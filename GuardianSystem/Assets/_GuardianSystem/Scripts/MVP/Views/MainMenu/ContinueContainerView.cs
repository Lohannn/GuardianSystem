using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContinueContainerView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI dayCounter;
    [SerializeField] private Image popularityPreview;
    [SerializeField] private Sprite goodPopularityIcon;
    [SerializeField] private Sprite badPopularityIcon;

    private void Awake()
    {
        PlayerData playerData = SaveData.LoadGame();

        if (playerData.lastDayNumber <= 0)
        {
            gameObject.SetActive(false);
        }
        else
        {
            dayCounter.text = $"Dia {playerData.lastDayNumber + 1}";

            popularityPreview.sprite = playerData.lastPopularityValue <= 45 ? badPopularityIcon : goodPopularityIcon;
        }
    }
}
