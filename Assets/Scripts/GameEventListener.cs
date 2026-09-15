using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{
    [Tooltip("The game event to listen to.")]
    public GameEvent Event;

    [Tooltip("The response to invoke when the event is raised.")]
    public UnityEvent Response;

    private void OnEnable()
    {
        if (Event != null)
        {
            Event.RegisterListener(this);
        }
    }
    private void OnDisable()
    {
        if (Event != null)
        {
            Event.UnregisterListener(this);
        }
    }

    public void OnEventRaised()
    {
        Response?.Invoke();
    }

    
}
