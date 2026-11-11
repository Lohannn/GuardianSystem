using UnityEngine;

public static class UnitMovementCalculator
{
    public static float CalculateTravelTime(Vector2 origin, Vector2 destination, float speed)
    {
        float distance = Vector2.Distance(origin, destination);

        if (GameManager.Instance.CurrentGameState == GameState.Utopia)
        {
            return 0;
        }

        return distance / speed;
    }
}
