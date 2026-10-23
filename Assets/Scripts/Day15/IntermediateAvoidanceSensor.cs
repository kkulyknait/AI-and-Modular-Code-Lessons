using UnityEngine;

[RequireComponent(typeof(IntermediateAgentContext))]
public class IntermediateAvoidanceSensor : MonoBehaviour
{
    #region Inspector Variables

    [SerializeField]
    private float detectionDistance = 2f;

    [SerializeField]
    private float sensorRadius = 0.4f;

    [SerializeField]
    private float sensorHeight = 0.5f;

    [SerializeField]
    private LayerMask blockingMask;

    [SerializeField]
    private bool canCrossPits = true;

    #endregion

    #region Private Variables

    private IntermediateAgentContext context;

    #endregion

    #region Unity Methods

    /// <summary>
    /// Gets the IntermediateAgentContext required by the sensor.
    /// </summary>
    private void Awake()
    {
        context = GetComponent<IntermediateAgentContext>();
    }

    /// <summary>
    /// Evaluates the local directions available to the agent.
    /// </summary>
    private void Update()
    {
        context.ClearAvailableDirections();

        // Test the forward direction.

        // Test the right direction.

        // Test the backward direction.

        // Test the left direction.
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Adds a direction when it is not blocked.
    /// </summary>
    /// <param name="direction">The world-space direction to test.</param>
    private void AddDirectionIfAvailable(Vector3 direction)
    {
        if (!IsDirectionAvailable(direction))
        {
            return;
        }

        context.AddAvailableDirection(direction);
    }

    /// <summary>
    /// Uses a SphereCast to determine whether a direction is available.
    /// </summary>
    /// <param name="direction">The world-space direction to test.</param>
    /// <returns>True when the direction is available.</returns>
    private bool IsDirectionAvailable(Vector3 direction)
    {
        Vector3 origin = transform.position + Vector3.up * sensorHeight;

        bool hasHit = Physics.SphereCast(
            origin,
            sensorRadius,
            direction.normalized,
            out RaycastHit hit,
            detectionDistance,
            blockingMask,
            QueryTriggerInteraction.Collide);

        if (!hasHit)
        {
            return true;
        }

        if (canCrossPits && IsPit(hit.collider))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Determines whether a collider or one of its parents uses the Pit tag.
    /// </summary>
    /// <param name="hitCollider">The collider detected by the SphereCast.</param>
    /// <returns>True when the detected object is a Pit.</returns>
    private bool IsPit(Collider hitCollider)
    {
        Transform currentTransform = hitCollider.transform;

        while (currentTransform != null)
        {
            if (currentTransform.CompareTag("Pit"))
            {
                return true;
            }

            currentTransform = currentTransform.parent;
        }

        return false;
    }

    #endregion
}
