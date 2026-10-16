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
        foreach (Collider nearbyCollider in nearbyColliders)
        {
            Transform perceivedTransform =
                GetTaggedTransform(nearbyCollider.transform);
            if (perceivedTransform == null || perceivedTransform.root == transform.root)
            {
                continue;
            }
            if (!IsInsideFieldOfView(perceivedTransform) ||
                !HasLineOfSight(perceivedTransform))
            {
                continue;
            }
            context.AddPerceivedObject(perceivedTransform);
        }
        
    }

    private void OnDrawGizmosSelected()
    {
        // TODO: Draw the perception radius.
        // TODO: Draw the left and right field-of-view boundaries.

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, perceptionRadius);

        Vector3 leftBoundary =
            Quaternion.Euler(0f, -fieldOfViewAngle * 0.5f, 0f) * transform.forward;
        Vector3 rightBoundary =
            Quaternion.Euler(0f, fieldOfViewAngle * 0.5f, 0f) * transform.forward;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(
            transform.position,
            transform.position + leftBoundary * perceptionRadius);
        Gizmos.DrawLine(
            transform.position,
            transform.position + rightBoundary * perceptionRadius);

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
        Vector3 directionToCandidate = candidate.position - transform.position;
        directionToCandidate.y = 0f;  // Ignore vertical difference

        if (directionToCandidate.sqrMagnitude <= 0f)
        {
            return true; // Candidate is at the same position as the sensor
        }

        directionToCandidate.Normalize();

        float minimumDot = Mathf.Cos(fieldOfViewAngle * 0.5f * Mathf.Deg2Rad);
        float forwardDot = Vector3.Dot(transform.forward, directionToCandidate);

        return forwardDot >= minimumDot;
                 
    }

    /// <summary>
    /// Returns true when no obstruction blocks the candidate.
    /// </summary>
    /// <param name="candidate">The candidate transform.</param>
    /// <returns>True when the candidate has a clear line of sight.</returns>
    private bool HasLineOfSight(Transform candidate)
    {
        // TODO: Raycast from eye height toward the candidate.
        Vector3 origin = transform.position + Vector3.up * eyeHeight;
        Vector3 destination = candidate.position + Vector3.up * eyeHeight;
        Vector3 direction = destination - origin;
        float distance = direction.magnitude;

        if (distance <= 0f)
        {
            return true;
        }

        direction.Normalize();
        Debug.DrawRay(origin, direction * distance, Color.cyan);
        if (!Physics.Raycast(
            origin, 
            direction, 
            out RaycastHit hit,
            distance,
            perceptionMask | obstructionMask,
            QueryTriggerInteraction.Collide))
        {
            return false;
        }

        return hit.transform.root == candidate.root;
              
    }

    /// <summary>
    /// Finds the tagged transform that represents a perceived object.
    /// </summary>
    /// <param name="candidate">The transform returned by the physics query.</param>
    /// <returns>The tagged root or parent transform, or null if no supported tag is found.</returns>
    private Transform GetTaggedTransform(Transform candidate)
    {
        Transform current = candidate;

        while(current != null)
        {
            if (current.CompareTag("Player") || 
                current.CompareTag("AIAgent") ||
                current.CompareTag("Key") ||
                current.CompareTag("Powerup") ||
                current.CompareTag("Chest")  ||
                current.CompareTag("Hazard"))
            {
                return current;
            }
            current = current.parent;
        }
        return null;
    }

    #endregion
}
