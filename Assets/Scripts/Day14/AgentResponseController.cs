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
        // TODO: Get AgentContext and Rigidbody, then configure top-down movement.
    }

    private void FixedUpdate()
    {
        // TODO: Clear the prior response target before evaluating this frame.
        // TODO: Get the contextual response direction.
        // TODO: Use the avoidance direction while IsAvoiding is true.
        // TODO: Rotate and move with the Rigidbody.
    }

    private void Update()
    {
        // TODO: Draw a magenta line to ResponseTarget when it is not null.
    }
    #endregion

    #region Protected Methods

    protected abstract Vector3 GetResponseDirection();

    protected Vector3 GetDirectionTo(Transform target)
    {
        // TODO: Return the horizontal direction toward target.

        return Vector3.zero;
    }

    protected Vector3 GetDirectionAwayFrom(Transform target)
    {
        // TODO: Return the horizontal direction away from target.

        return Vector3.zero;
    }

    protected bool HasRecentPerception()
    {
        // TODO: Check LastPerceivedObject and memoryDuration.

        return false;
    }

    protected Vector3 GetDirectionToLastPerceivedPosition()
    {
        // TODO: Return the horizontal direction to LastPerceivedPosition.

        return Vector3.zero;
    }

    protected void SetResponseTarget(Transform target)
    {
        // TODO: Store ResponseTarget and HasResponseTarget.
    }
    #endregion
}
