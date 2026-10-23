using UnityEngine;

public class IntermediateKeyPickup : MonoBehaviour
{
    #region Unity Methods

    /// <summary>
    /// Gives the Key to an intermediate agent entering the trigger.
    /// </summary>
    /// <param name="other">The collider entering the trigger.</param>
    private void OnTriggerEnter(Collider other)
    {
        IntermediateAgentContext context = other.GetComponentInParent<IntermediateAgentContext>();

        if (context == null)
        {
            return;
        }

        context.HasKey = true;
        gameObject.SetActive(false);
    }

    #endregion
}
