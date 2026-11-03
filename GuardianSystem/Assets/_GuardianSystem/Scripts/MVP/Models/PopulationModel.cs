using UnityEngine;

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

        if (amount > 0)
        {
            Debug.Log($"Popularity increased by {amount}. New popularity: {Popularity}");
            //feedback de aceitação ganha aqui
        }
        else if (amount < 0)
        {
            Debug.Log($"Popularity decreased by {Mathf.Abs(amount)}. New popularity: {Popularity}");
            //feedback de perdida ganha aqui
        }
    }
}
