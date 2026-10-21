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

    [SerializeField]
    private float sensorRadius = 0.4f;

    [SerializeField]
    private LayerMask avoidanceMask;

    [SerializeField]
    private float holdTime = 0.3f;

    #endregion

    #region Private Variables

    private AgentContext context;

    private float holdTimer;

    private Vector3 heldDirection;

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
        bool hasHit = Physics.SphereCast(
            sensorOrigin,
            sensorRadius,
            sensorDirection,
            out RaycastHit hit,
            detectionDistance,
            avoidanceMask,
            QueryTriggerInteraction.Collide);
        //  ProjectOnPlane keeps only the part of our heading that runs along the wall,
        //  so the agent slides past it
        if (hasHit)
        {
            Vector3 wallNormal = hit.normal;
            wallNormal.y = 0f;

            Vector3 slideDirection =
                Vector3.ProjectOnPlane(sensorDirection, wallNormal);
            slideDirection.y = 0f;

            if (slideDirection.sqrMagnitude < 0.01f)
            {
                slideDirection = transform.right;
            }
            heldDirection = slideDirection.normalized + wallNormal.normalized * 0.5f;
            holdTimer = holdTime;

        }
        else
        {
            holdTimer -= Time.deltaTime;
        }

            context.IsAvoiding = holdTimer > 0f;
            context.AvoidanceDirection = context.IsAvoiding ? heldDirection : Vector3.zero;

        //context.IsAvoiding = hasHit;
        //context.AvoidanceDirection = hasHit ? transform.right : Vector3.zero;

        //// TODO: Raycast for hazards first.
        //bool hasHit = Physics.Raycast(sensorOrigin, sensorDirection, out RaycastHit hitInfo, detectionDistance);

        //// TODO: Store IsAvoiding and AvoidanceDirection.
        //if (!hasHit)
        //{
        //    context.IsAvoiding = false;
        //    context.AvoidanceDirection = Vector3.zero;

        //    return;
        //}

        //DetectionSource source = hitInfo.collider.GetComponent<DetectionSource>();
        
        //if (!ShouldAvoid(source))
        //{
        //    context.IsAvoiding = false;
        //    context.AvoidanceDirection = Vector3.zero;
            
        //    return;
        //}

        //context.IsAvoiding = true;

        ////We're arbitrarily choosing to avoid to the right, but you could implement a more complex avoidance strategy here.
        //context.AvoidanceDirection = transform.right;

        //// TODO: Extend the check to include obstacles.
    }
    private void OnDrawGizmos()
    {
        // TODO: Draw the red detection ray.
        Vector3 sensorOrigin = transform.position + Vector3.up * sensorHeight;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(sensorOrigin, sensorOrigin + transform.forward * detectionDistance);

        Vector3 sensorEnd = sensorOrigin + transform.forward * detectionDistance;

        Gizmos.DrawWireSphere(sensorOrigin, sensorRadius);
        Gizmos.DrawWireSphere(sensorEnd, sensorRadius);

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
    //private bool ShouldAvoid(DetectionSource source)
    //{
    //    // TODO: Begin with hazards, then extend to obstacles.
    //    if (source == null)
    //    {
    //        return false;
    //    }

    //    return source.Type == DetectionType.Hazard || source.Type == DetectionType.Obstacle;
    //}

    #endregion
}
