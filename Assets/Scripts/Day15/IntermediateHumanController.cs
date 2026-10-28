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
        if (context.VisibleKey != null)
        {
            SetResponseTarget(context.VisibleKey);
            return GetDirectionTo(context.VisibleKey);
        }
        // Discover and follow a visible Chest while carrying the Key.
        if (context.HasKey && context.VisibleChest != null)
        {
            if (context.PersistentTarget != context.VisibleChest)
            {
                context.SetPersistentTarget(context.VisibleChest);
                context.ClearCommittedDirection();
            }
            SetResponseTarget(context.VisibleChest);
            return GetDirectionTo(context.VisibleChest);
        }
        // Continue following the remembered Chest.
        if (context.HasKey && context.HasPersistentTarget
            && context.PersistentTarget != null)
        {
            SetResponseTarget(context.PersistentTarget);
            return GetDirectionTo(context.PersistentTarget);

        }

        // Clear the remembered Chest after the Key is lost.
        if (!context.HasKey && context.HasPersistentTarget)
        {
            context.ClearPersistentTarget();
        }
        // Respond to a visible Coin.
        if (context.VisibleCoin != null)
        {
            SetResponseTarget(context.VisibleCoin);
            return GetDirectionTo(context.VisibleCoin);
        }


        // Maintain and advance the remembered Coin search.
        EnsureCoinSearchTarget();
        AdvanceCoinSearchIfRequired();
        if(context.HasSearchTarget)
        {
            return GetDirectionTo(context.SearchTargetPosition);
        }
        // Return zero so the base controller wanders.
        return Vector3.zero;
    }

    #endregion
}
