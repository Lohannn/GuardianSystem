using UnityEngine;

public static class UnitMovementCalculator
{
    public static float CalculateTravelTime(Vector2 origin, Vector2 destination, float speed)
    {
        Vector2 originScreen = RectTransformUtility.WorldToScreenPoint(Camera.main, origin);
        Vector2 destinationScreen = RectTransformUtility.WorldToScreenPoint(Camera.main, destination);

        float distance = Vector2.Distance(originScreen, destinationScreen);

        if (GameManager.Instance.CurrentGameState == GameState.Utopia)
        {
            return 0;
        }

        return distance / speed;
    }
}
