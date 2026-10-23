using UnityEngine;

public class IntermediateChestInteraction : MonoBehaviour
{
    #region Private Variables

    private bool isOpen;

    #endregion

    #region Unity Methods

    /// <summary>
    /// Opens the Chest when an intermediate agent carrying the Key enters.
    /// </summary>
    /// <param name="other">The collider entering the trigger.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (isOpen)
        {
            return;
        }

        IntermediateAgentContext context = other.GetComponentInParent<IntermediateAgentContext>();

        if (context == null || !context.HasKey)
        {
            return;
        }

        isOpen = true;
        context.HasKey = false;
    }

    #endregion
}
