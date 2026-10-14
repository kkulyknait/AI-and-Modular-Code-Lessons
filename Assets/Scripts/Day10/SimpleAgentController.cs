using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AgentContext))]
public class SimpleAgentController : MonoBehaviour
{
    #region Inspector Variables

    [SerializeField]
    private float moveSpeed = 3f;

    [SerializeField]
    private float stopDistance = 0.75f;

    [SerializeField]
    private float turnSpeed = 5f;

    [SerializeField]
    private ThreatResponse threatResponse = ThreatResponse.Flee;

    #endregion

    #region Private Variables

    private AgentContext context;

    private Rigidbody rigidBody;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        context = GetComponent<AgentContext>();

        rigidBody = GetComponent<Rigidbody>();

        rigidBody.useGravity = false;
        rigidBody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ;
    }

    private void FixedUpdate()
    {
        Vector3 movementDirection = SelectMovementDirection();
        movementDirection.y = 0f;

        if(movementDirection.sqrMagnitude < 0f)
        {
            return;
        }

        movementDirection.Normalize();

        Quaternion targetRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
        Quaternion nextRotation = Quaternion.Slerp(rigidBody.rotation, targetRotation, 
            turnSpeed * Time.fixedDeltaTime);

        Vector3 nextPosition = rigidBody.position + movementDirection * 
            moveSpeed * Time.fixedDeltaTime;
        
        rigidBody.MoveRotation(nextRotation);
        rigidBody.MovePosition(nextPosition);

    }

    private Vector3 SelectMovementDirection()
    { 
        if (context.IsAvoiding)
        {
            return context.AvoidanceDirection.normalized;
        }

        if (context.IsThreatDetected)
        {
            if (threatResponse == ThreatResponse.Chase)
            {
                return context.ThreatDistance <= stopDistance ? Vector3.zero
                    : context.ThreatDirection.normalized;
            }
            return -context.ThreatDirection.normalized;
        }

        if (context.CurrentTarget == null)
        {
            return Vector3.zero;
        }

        Vector3 targetPosition = context.CurrentTarget.transform.position;
        Vector3 direction = targetPosition - rigidBody.position;
        direction.y = 0f;

        if(direction.magnitude <= stopDistance)
        {
            return Vector3.zero;
        }
        return direction;
    }
    

    #endregion
}
