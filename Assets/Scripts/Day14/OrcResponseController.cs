using UnityEngine;
public class OrcResponseController : AgentResponseController
{
    #region Protected Methods
    protected override Vector3 GetResponseDirection()
    {
        return Vector3.zero;
    }
    #endregion
}
