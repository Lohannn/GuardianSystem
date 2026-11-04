using UnityEngine;

public static class UnitMovementCalculator
{
    public static float CalculateTravelTime(Vector2 origin, Vector2 destination, float speed)
    {
        float distance = Vector2.Distance(origin, destination);
        
        return distance / speed;
    }
}
