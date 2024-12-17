
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerConnectionEvent : ScriptableObject
{
    private readonly List<PlayerConnectionListener > eventListeners = new List<PlayerConnectionListener>();

    public void Raise(int playerCount, ulong playerID,Color playerColor)
    {
        Debug.Log("eventListener: " + eventListeners.Count);
        for (int i = eventListeners.Count -1; i >= 0; i--)
            eventListeners[i].OnEventRaised(playerCount, playerID,playerColor);
    }

    public void RegisterListener(PlayerConnectionListener listener)
    {
        if (!eventListeners.Contains(listener))
            eventListeners.Add(listener);
    }

    public void UnregisterListener(PlayerConnectionListener listener)
    {
        if (eventListeners.Contains(listener))
            eventListeners.Remove(listener);
    }
}