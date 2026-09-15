using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    // create inspector slot for scriptable object
    [SerializeField] private InputReader inputReader;

    private void OnEnable()
    {
        // subscribe to middleman's events when the object turns on
        if (inputReader != null)
        {
            inputReader.NavigateEvent += HandleNavigation;
            inputReader.SubmitEvent += HandleSubmit;
        }
    }
    private void onDisable()
    {
        if (inputReader != null)
        {
            inputReader.NavigateEvent -= HandleNavigation;
            inputReader.SubmitEvent -= HandleSubmit;
        }
    }

    //  Define what actually happens when inputs are received
    private void HandleNavigation(Vector2 direction)
    {
        Debug.Log($"Moving UI selection by {direction}");
        //  logic to highlight next UI element goes here
    }

    private void HandleSubmit()
    {
        Debug.Log("UI option selection");
        //  logic to click the UI button goes here
    }

}
