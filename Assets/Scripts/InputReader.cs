using UnityEngine;
using UnityEngine.InputSystem;
using System;  // Stardard C# Actions


[CreateAssetMenu(fileName = "InputReader", menuName = "Modular Assets/InputReader")]
public class InputReader : ScriptableObject

{
    // create generic action for input events
    public event Action<Vector2> NavigateEvent;
    public event Action SubmitEvent;

    //  create reference to C# class Unity created for us
    private GameInput _gameInput;

    private void OnEnable()
    {
        if (_gameInput == null)
        {
            _gameInput = new GameInput();
            // subscribe to our input system's events
            _gameInput.UI.Navigate.performed += HandleNavigate;
            _gameInput.UI.Submit.performed += HandleSubmit;
        }
        // turn on action map
        _gameInput.UI.Enable();

    }
    private void OnDisable()
    {
        // to prevent memory leaks , unsubscribe from events when disabled
        _gameInput.UI.Navigate.performed -= HandleNavigate;
        _gameInput.UI.Submit.performed -= HandleSubmit;

        _gameInput.UI.Disable();
    }

    // When the input system hears a Navigate event, it will call this method
    private void HandleNavigate(InputAction.CallbackContext context)
    {
        // ?.Invoke checks to see if any scripts are actually listening before shouting
        NavigateEvent?.Invoke(context.ReadValue<Vector2>());

    }

    // when the input system hears Submit, run this method
    private void HandleSubmit(InputAction.CallbackContext context)
    {
        SubmitEvent?.Invoke();
    }


}
