using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewGameEvent", menuName = "Modular Assets/Game Event")]
public class GameEvent : ScriptableObject
{
    //  Track every listener subscribed to this event
    private readonly List<GameEventListener> _listeners = new List<GameEventListener>();

    [ContextMenu("Raise")]
    public void Raise()
    {
        // work backwards through the list in case any listeners are removed during the loop
        for (int i= _listeners.Count -1; i >= 0; i--)
        {
            _listeners[i].OnEventRaised();
        }
    }
    public void RegisterListener(GameEventListener listener)
    {
        if (!_listeners.Contains(listener))
        {
            _listeners.Add(listener);
        }
        
    }
    public void UnregisterListener(GameEventListener listener)
    {
        if (_listeners.Contains(listener))
        {
            _listeners.Remove(listener);
        }
    }

}
