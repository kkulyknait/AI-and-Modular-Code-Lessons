using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Day22_AgentContext))]
[RequireComponent(typeof(Day22_ContextSensor))]
public class Day22_PathfindingAgent : MonoBehaviour
{
    #region Inspector Variables

    [SerializeField]
    private Day22_PathGrid pathGrid;

    [SerializeField]
    private float moveSpeed = 3f;

    [SerializeField]
    private float turnSpeed = 8f;

    [SerializeField]
    private float nodeReachDistance = 0.2f;

    #endregion

    #region Private Variables

    private Day22_AgentContext context;
    private Day22_ContextSensor contextSensor;
    private Rigidbody rigidBody;

    #endregion

    #region Unity Methods

    /// <summary>
    /// Gets required components and configures top-down Rigidbody movement.
    /// </summary>
    private void Awake()
    {
        context = GetComponent<Day22_AgentContext>();
        contextSensor = GetComponent<Day22_ContextSensor>();
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.useGravity = false;
        rigidBody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    /// <summary>
    /// Requests the first A* path after scene initialization is complete.
    /// </summary>
    private void Start()
    {
        // Request the first path from the Human to the Chest.
    }

    /// <summary>
    /// Refreshes context, follows the current path, and locally adjusts movement.
    /// </summary>
    private void FixedUpdate()
    {
        // Sense before deciding how to move.

        // Request another path when the Chest enters a different Goal node.

        // Stop when no usable path exists.

        // Advance after reaching the current path node.

        // Stop after reaching the end of the path.

        // Calculate the desired direction toward the current path node.

        // Select the available local direction that best supports the path.

        // Rotate and move the Human.
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Runs A* and stores the generated path in the agent context.
    /// </summary>
    private void RequestPath()
    {
        // Run A* and store the generated path in the agent context.
    }

    /// <summary>
    /// Selects the locally available direction that best supports the desired path direction.
    /// </summary>
    private Vector3 SelectBestAvailableDirection(Vector3 desiredDirection)
    {
        // Stop when no local directions are available.

        // Keep a useful committed direction.

        // Compare each available direction against the desired path direction.

        // Permit reversal when no other direction is available.

        // Commit to the selected direction.
        return Vector3.zero;
    }

    private bool IsDirectionAvailable(Vector3 direction)
    {
        foreach (Vector3 availableDirection in context.AvailableDirections)
        {
            if (Vector3.Dot(availableDirection.normalized, direction.normalized) > 0.99f)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsReverseDirection(Vector3 candidateDirection)
    {
        if (!context.HasCommittedDirection)
        {
            return false;
        }

        return Vector3.Dot(context.CurrentDirection.normalized, candidateDirection.normalized) < -0.9f;
    }

    private void RotateAgent(Vector3 movementDirection)
    {
        Quaternion targetRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
        Quaternion nextRotation = Quaternion.Slerp(rigidBody.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);
        rigidBody.MoveRotation(nextRotation);
    }

    private void MoveAgent(Vector3 movementDirection)
    {
        Vector3 nextPosition = rigidBody.position + movementDirection * moveSpeed * Time.fixedDeltaTime;
        rigidBody.MovePosition(nextPosition);
    }

    #endregion
}
