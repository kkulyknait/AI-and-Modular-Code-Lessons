using UnityEngine;

[RequireComponent(typeof(AgentContext))]
public class PerceptionSensor : MonoBehaviour
{
    #region Inspector Variables

    [SerializeField]
    private float perceptionRadius = 6f;

    [SerializeField]
    private float fieldOfViewAngle = 90f;

    [SerializeField]
    private float eyeHeight = 0.5f;

    [SerializeField]
    private LayerMask perceptionMask;

    [SerializeField]
    private LayerMask obstructionMask;

    #endregion

    #region Private Variables

    private AgentContext context;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        context = GetComponent<AgentContext>();
    }

    private void Update()
    {
        context.ClearPerception();

        Collider[] nearbyColliders = Physics.OverlapSphere(
            transform.position,
            perceptionRadius,
            perceptionMask,
            QueryTriggerInteraction.Collide);

        // TODO: Filter candidates by field of view.
        // TODO: Confirm line of sight with Physics.Raycast.
        // TODO: Resolve each candidate to its tagged root object.
        // TODO: Add visible objects to AgentContext.
    }

    private void OnDrawGizmosSelected()
    {
        // TODO: Draw the perception radius.
        // TODO: Draw the left and right field-of-view boundaries.
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Returns true when a candidate is inside the sensor's field of view.
    /// </summary>
    /// <param name="candidate">The candidate transform.</param>
    /// <returns>True when the candidate is inside the configured field of view.</returns>
    private bool IsInsideFieldOfView(Transform candidate)
    {
        // TODO: Use Vector3.Dot to compare the forward direction with the direction to the candidate.
        return false;
    }

    /// <summary>
    /// Returns true when no obstruction blocks the candidate.
    /// </summary>
    /// <param name="candidate">The candidate transform.</param>
    /// <returns>True when the candidate has a clear line of sight.</returns>
    private bool HasLineOfSight(Transform candidate)
    {
        // TODO: Raycast from eye height toward the candidate.
        return false;
    }

    /// <summary>
    /// Finds the tagged transform that represents a perceived object.
    /// </summary>
    /// <param name="candidate">The transform returned by the physics query.</param>
    /// <returns>The tagged root or parent transform, or null if no supported tag is found.</returns>
    private Transform GetTaggedTransform(Transform candidate)
    {
        
        return null;
    }

    #endregion
}
