using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class HumanController : MonoBehaviour
{
    #region Inspector Variables

    [SerializeField]
    private float moveSpeed = 5f;

    #endregion

    #region Private Variables

    private CharacterController characterController;

    private Vector2 moveInput;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Vector3 movement = new Vector3(moveInput.x, 0f, moveInput.y);

        characterController.Move(movement.normalized * moveSpeed * Time.deltaTime);
    }

    #endregion

    #region Input Methods

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    #endregion
}
