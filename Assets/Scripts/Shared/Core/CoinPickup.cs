using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    #region Unity Methods

    /// <summary>
    /// Disables the Coin when an intermediate agent enters its trigger.
    /// </summary>
    /// <param name="other">The collider that entered the trigger.</param>
    private void OnTriggerEnter(Collider other)
    {
        IntermediateAgentContext context = other.GetComponentInParent<IntermediateAgentContext>();

        if (context == null)
        {
            return;
        }

        gameObject.SetActive(false);
    }

    #endregion
}
