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
        context = GetComponent<AgentContext>();
    }

    private void Update()
    {
        // TODO: Calculate the sensor origin and forward direction.
        Vector3 sensorOrigin = transform.position + Vector3.up * sensorHeight;
        Vector3 sensorDirection = transform.forward;

        // TODO: Draw the runtime debug ray.
        Debug.DrawRay(sensorOrigin, sensorDirection * detectionDistance, Color.red);

        // TODO: Raycast for hazards first.
        bool hasHit = Physics.Raycast(sensorOrigin, sensorDirection, out RaycastHit hitInfo, detectionDistance);

        // TODO: Store IsAvoiding and AvoidanceDirection.
        if (!hasHit)
        {
            context.IsAvoiding = false;
            context.AvoidanceDirection = Vector3.zero;

            return;
        }

        DetectionSource source = hitInfo.collider.GetComponent<DetectionSource>();
        
        if (!ShouldAvoid(source))
        {
            context.IsAvoiding = false;
            context.AvoidanceDirection = Vector3.zero;
            
            return;
        }

        context.IsAvoiding = true;

        //We're arbitrarily choosing to avoid to the right, but you could implement a more complex avoidance strategy here.
        context.AvoidanceDirection = transform.right;

        // TODO: Extend the check to include obstacles.
    }
    private void OnDrawGizmos()
    {
        // TODO: Draw the red detection ray.
        Vector3 sensorOrigin = transform.position + Vector3.up * sensorHeight;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(sensorOrigin, sensorOrigin + transform.forward * detectionDistance);

        // TODO: Draw the green avoidance direction while avoiding.
        if (context == null || !context.IsAvoiding)
        {
            return;
        }

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position,
                        transform.position +
                        context.AvoidanceDirection.normalized *
                        avoidanceDistance);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Returns true when the detected source should be avoided.
    /// </summary>
    /// <param name="source">The source detected by the forward ray.</param>
    /// <returns>True when the source is a hazard or obstacle.</returns>
    private bool ShouldAvoid(DetectionSource source)
    {
        // TODO: Begin with hazards, then extend to obstacles.
        if (source == null)
        {
            return false;
        }

        return source.Type == DetectionType.Hazard || source.Type == DetectionType.Obstacle;
    }

    #endregion
}
