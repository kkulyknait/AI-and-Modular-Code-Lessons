using UnityEngine;

[RequireComponent(typeof(AgentContext))]
public class AvoidanceSensor : MonoBehaviour
{
    #region Inspector Variables

    [SerializeField]
    private float detectionDistance = 2f;

    [SerializeField]
    private float sensorHeight = 0.5f;

    [SerializeField]
    private float avoidanceDistance = 1.5f;

    #endregion

    #region Private Variables

    private AgentContext context;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        // TODO: Get the AgentContext component.
    }

    private void Update()
    {
        // TODO: Calculate the sensor origin and forward direction.
        // TODO: Draw the runtime debug ray.
        // TODO: Raycast for hazards first.
        // TODO: Store IsAvoiding and AvoidanceDirection.
        // TODO: Extend the check to include obstacles.
    }

    private void OnDrawGizmos()
    {
        // TODO: Draw the red detection ray.
        // TODO: Draw the green avoidance direction while avoiding.
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Returns true when the detected source should be avoided.
    /// </summary>
    /// <param name="source">The source detected by the forward ray.</param>
    /// <returns>True when the source is a hazard or obstacle.</returns>
    private bool ShouldAvoid(
        DetectionSource source)
    {
        // TODO: Begin with hazards, then extend to obstacles.
        return false;
    }

    #endregion
}
