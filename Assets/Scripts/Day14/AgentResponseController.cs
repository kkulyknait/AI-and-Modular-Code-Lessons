using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AgentContext))]
public abstract class AgentResponseController : MonoBehaviour
{
    #region Inspector Variables

    [SerializeField]
    private float moveSpeed = 3f;

    [SerializeField]
    private float turnSpeed = 8f;

    [SerializeField]
    private float stopDistance = 0.75f;

    [SerializeField]
    private float memoryDuration = 2f;

    #endregion

    #region Protected Variables

    protected AgentContext context;

    protected Rigidbody rigidBody;

    #endregion

    #region Unity Methods

    protected virtual void Awake()
    {
        context = GetComponent<AgentContext>();
        rigidBody = GetComponent<Rigidbody>();

        rigidBody.useGravity = false;
        rigidBody.constraints =
            RigidbodyConstraints.FreezePositionY |
            RigidbodyConstraints.FreezePositionX |
            RigidbodyConstraints.FreezePositionZ;

    }


    private void FixedUpdate()
    {
        rigidBody.linearVelocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;

        context.ResponseTarget = null;
        context.HasResponseTarget = false;

        Vector3 responseDirection = GetResponseDirection();

        //  If IsAvoiding is true, use AvoidanceDirection
        //  Otherwise, use responseDirection

        Vector3 movementDirection =
            context.IsAvoiding ? context.AvoidanceDirection : responseDirection;

        movementDirection.y = 0f;

        if (movementDirection.sqrMagnitude <= 0f)
        {
            return;
        }
        if (!context.IsAvoiding && context.ResponseTarget != null)
        {
            Vector3 toTarget = context.ResponseTarget.position - rigidBody.position;
            toTarget.y = 0f;

            if (toTarget.magnitude <= stopDistance)
            {
                return;
            }
        }
        movementDirection.Normalize();

        Quaternion targetRotation =
            Quaternion.LookRotation(movementDirection, Vector3.up);

        Quaternion nextRotation = Quaternion.Slerp(
            rigidBody.rotation,
            targetRotation,
            turnSpeed * Time.fixedDeltaTime);

        rigidBody.MoveRotation(nextRotation);
        rigidBody.linearVelocity = movementDirection * moveSpeed;

    }

    private void Update()
    {
        if (context.ResponseTarget != null)
        {
            Debug.DrawLine(transform.position, context.ResponseTarget.position, Color.magenta);
        }
    }
    #endregion

    #region Protected Methods

    protected abstract Vector3 GetResponseDirection();

    protected Vector3 GetDirectionTo(Transform target)
    {
        if (target == null)
        {
            return Vector3.zero;
        }
        Vector3 direction = target.position - rigidBody.position;
        direction.y = 0f;
        return direction;
        
    }

    protected Vector3 GetDirectionAwayFrom(Transform target)
    {
        if (target == null)
        {
            return Vector3.zero;
        }
        Vector3 direction = rigidBody.position - target.position;
        direction.y = 0f;
        return direction;

    }

    protected bool HasRecentPerception()
    {
        return context.LastPerceivedObject != null &&
            Time.time - context.LastPerceivedTime <= memoryDuration;
    }

    protected Vector3 GetDirectionToLastPerceivedPosition()
    {
        Vector3 direction = context.LastPerceivedPosition - rigidBody.position;
        direction.y = 0f;
        return direction;
    }

    protected void SetResponseTarget(Transform target)
    {
        context.ResponseTarget = target;
        context.HasResponseTarget = target != null;
    }
    #endregion
}
