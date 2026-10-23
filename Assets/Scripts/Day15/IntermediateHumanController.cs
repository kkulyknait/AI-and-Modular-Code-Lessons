using UnityEngine;

public class IntermediateHumanController : IntermediateAgentController
{
    #region Protected Methods

    /// <summary>
    /// Selects the Human's desired movement direction from its ordered rules.
    /// </summary>
    /// <returns>The desired movement direction.</returns>
    protected override Vector3 GetDesiredDirection()
    {
        // Respond to a visible Key.

        // Discover and follow a visible Chest while carrying the Key.

        // Continue following the remembered Chest.

        // Clear the remembered Chest after the Key is lost.

        // Respond to a visible Coin.

        // Maintain and advance the remembered Coin search.

        // Return zero so the base controller wanders.
        return Vector3.zero;
    }

    #endregion
}
