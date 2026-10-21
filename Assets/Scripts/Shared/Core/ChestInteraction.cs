using UnityEngine;
public class ChestInteraction : MonoBehaviour
{
    #region Private Variables

    private bool isOpen;

    #endregion

    #region Public Properties

    public bool IsOpen => isOpen;

    #endregion

    #region Unity Methods

    private void OnTriggerEnter(Collider other)
    {
        if (isOpen)
        {
            return;
        }

        AgentContext context = other.GetComponentInParent<AgentContext>();

        if (context == null || !context.HasKey)
        {
            return;
        }

        isOpen = true;

        context.HasKey = false;

        Debug.Log($"{context.name} opened the Chest.", this);
    }
    #endregion
}
