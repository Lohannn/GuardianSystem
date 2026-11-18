using UnityEngine;

public enum PopularityLevel
{
    Bad,
    Neutral,
    Good
}

public class PopulationModel
{
    public float Popularity { get; private set; }

    public PopulationModel(float initialPopularity = 50)
    {
        Popularity = Mathf.Clamp(initialPopularity, 0, 100);
    }

    public void AdjustPopularity(float amount)
    {
        Popularity = Mathf.Clamp(Popularity + amount, 0, 100);

        EventBus.OnPopularityValueChanged?.Invoke(amount > 0);
    }

    public void SetPopularity(float amount)
    {
        Popularity = Mathf.Clamp(Popularity + amount, 0, 100);
    }

    public PopularityLevel GetPopularityLevel()
    {
        switch (Popularity)
        {
            case > 60f:
                return PopularityLevel.Good;
            case > 45f:
                return PopularityLevel.Neutral;
            case <= 45f:
                return PopularityLevel.Bad;
            default:
                return PopularityLevel.Neutral;
        }
    }
}
