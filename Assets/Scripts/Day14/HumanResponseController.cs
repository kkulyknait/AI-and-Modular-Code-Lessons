using UnityEngine;
public class HumanResponseController : AgentResponseController
{
    #region Protected Methods
    protected override Vector3 GetResponseDirection()
    {
        if (context.VisiblePickup != null
            && context.VisiblePickup.CompareTag("Key"))
        {
            SetResponseTarget(context.VisiblePickup);
            return GetDirectionTo(context.VisiblePickup);
        }

        if(context.HasKey && 
            context.VisibleTarget != null &&
            context.VisibleTarget.CompareTag("Chest"))
        {
            SetResponseTarget(context.VisibleTarget);
            return GetDirectionTo(context.VisibleTarget);
        }

        if(context.VisibleAgent != null &&
            context.VisibleAgent.CompareTag("AIAgent"))
        {
            AgentContext other =
                context.VisibleAgent.GetComponent<AgentContext>();

            if (other != null && other.HasKey)
            {
                SetResponseTarget(context.VisibleAgent);
                return GetDirectionTo(context.VisibleAgent);
            }           
        }
        if(HasRecentPerception())
        {
            return GetDirectionToLastPerceivedPosition();
        }
        return Vector3.zero;

    }
    #endregion
}
