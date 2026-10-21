using UnityEngine;
public class HumanResponseController : AgentResponseController
{
    #region Protected Methods
    protected override Vector3 GetResponseDirection()
    {
        return Vector3.zero;
    }
    #endregion
}
