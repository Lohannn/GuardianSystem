using System;
using UnityEngine;

public enum EventWeight
{
    Simple, 
    Medium,
    Hard
}

public enum EventType
{
    Dispatch,
    Question
}

public enum UnitType
{
    Police,
    Firefighter,
    Paramedic,
    TrashCollector
}

[System.Serializable]
public struct EventAnswer
{
    public string answerText;
    public int popularityImpact;
}

[CreateAssetMenu(fileName = "New Event", menuName = "GS/Events/Event Data")]
public class EventData : ScriptableObject
{
    [Header("Event Details")]
    public EventWeight eventWeight;
    [TextArea(3, 5)] public string eventDescription;
    public Sprite eventImage;
    public EventType eventType;
    public float eventDuration = 15f;

    [Header("Dispatch Event")]
    public UnitType[] requiredUnits;
    public int popularityImpactOnSuccess;
    public int popularityImpactOnWrongUnits;

    [Header("Question Event")]
    public EventAnswer[] possibleAnswers;

    [Header("Event Impact")]
    public int popularityImpactOnTimeout;

    private void OnValidate()
    {
        if (eventType == EventType.Dispatch)
        {
            if (requiredUnits == null || requiredUnits.Length == 0)
            {
                Debug.LogWarning($"Event '{name}' is of type Dispatch but has no required units assigned.");
                Array.Resize(ref requiredUnits, 1);
            }
            else if (requiredUnits.Length > 2)
            {
                Debug.LogWarning($"Event '{name}' has more than 2 required units. Only the first 2 will be considered.");
                Array.Resize(ref requiredUnits, 2);
            }
        } else
        {
            if (possibleAnswers.Length > 4)
            {
                Debug.LogWarning($"Event '{name}' has more than 4 possible answers. Only the first 4 will be considered.");
                Array.Resize(ref possibleAnswers, 4);
            }
            else if (possibleAnswers == null || possibleAnswers.Length < 2)
            {
                Debug.LogWarning($"Event '{name}' has less than 2 possible answers. At least 2 are required.");
                Array.Resize(ref possibleAnswers, 2);
            }
        }
    }
}


