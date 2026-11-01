using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "EventDatabase", menuName = "GS/Events/Event Database")]
public class EventDatabase : ScriptableObject
{
    [SerializeField] private List<EventData> events;

    public List<EventData> Events => events;

    public List<EventData> GetEventsByType(EventWeight[] eventWeights)
    {
        return events.FindAll(e => 
            Array.Exists(eventWeights, weight => weight == e.eventWeight)
        );
    }
}
