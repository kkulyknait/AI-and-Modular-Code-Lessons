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
        if (context.CurrentTarget == null)
        {
            return;
        }

        Vector3 targetPosition = context.CurrentTarget.transform.position;

        Vector3 direction = targetPosition - rigidBody.position;

        direction.y = 0f;

        if (!context.IsAvoiding && direction.magnitude <= stopDistance)
        {
            return;
        }
        //  This is where I left off last class
        Vector3 nextPosition = rigidBody.position + direction.normalized * moveSpeed * Time.fixedDeltaTime;

        rigidBody.MovePosition(nextPosition);
    }

    #endregion
}
