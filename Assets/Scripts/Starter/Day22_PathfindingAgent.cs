using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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
        RequestPath();

    }

    /// <summary>
    /// Refreshes context, follows the current path, and locally adjusts movement.
    /// </summary>
    private void FixedUpdate()
    {
        // Sense before deciding how to move.
        contextSensor.RefreshAvailableDirections();

        // Request another path when the Chest enters a different Goal node.
        if (pathGrid.GoalNode != context.PreviousGoalNode)
        {
            RequestPath();
        }

        // Stop when no usable path exists.
        if (context.CurrentPath.Count == 0 || context.CurrentPathIndex >= 
            context.CurrentPath.Count)
        {
            return;
        }
        // Advance after reaching the current path node.
        Day22_GridNode currentPathNode = context.CurrentPath[context.CurrentPathIndex];
        Vector3 directionToNode = currentPathNode.WorldPosition - rigidBody.position;
        directionToNode.y = 0f;
        if (directionToNode.magnitude <= nodeReachDistance)
        {
            context.CurrentPathIndex++;
            context.ClearCommittedDirection();
        }
        // Stop after reaching the end of the path.
        if (context.CurrentPathIndex >= context.CurrentPath.Count)
        {
            return;
        }

        // Calculate the desired direction toward the current path node.
        currentPathNode = context.CurrentPath[context.CurrentPathIndex];
        Vector3 desiredDirection = currentPathNode.WorldPosition - rigidBody.position;
        desiredDirection.y = 0f;

        // Select the available local direction that best supports the path.
        Vector3 movementDirection = SelectBestAvailableDirection(desiredDirection);
        if(movementDirection.sqrMagnitude <= 0f)
        {
            return;
        }
        // Rotate and move the Human.
        RotateAgent(movementDirection);
        MoveAgent(movementDirection);

    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Runs A* and stores the generated path in the agent context.
    /// </summary>
    private void RequestPath()
    {
        // Run A* and store the generated path in the agent context.
        List<Day22_GridNode> path = pathGrid.RunSearch();
        context.SetPath(path, pathGrid.GoalNode);
    }

    /// <summary>
    /// Selects the locally available direction that best supports the desired path direction.
    /// </summary>
    private Vector3 SelectBestAvailableDirection(Vector3 desiredDirection)
    {
        // Stop when no local directions are available.
        if (context.AvailableDirections.Count == 0)
        {
            context.ClearAvailableDirections();
            return Vector3.zero;
        }
        Vector3 normalizedDesiredDirection = desiredDirection.normalized;

        // Keep a useful committed direction.
        if(context.HasCommittedDirection && IsDirectionAvailable(context.CurrentDirection))
        {
            if (Vector3.Dot(context.CurrentDirection, normalizedDesiredDirection) > 0.5f) 
            {
                return context.CurrentDirection;
            }
        }
        Vector3 bestDirection = Vector3.zero;
        float bestAlignment = float.NegativeInfinity;

        // Compare each available direction against the desired path direction.
        foreach (Vector3 candidateDirection in context.AvailableDirections)
        {
            if(IsReverseDirection(candidateDirection))
            {
                continue;
            }
            float alignment = Vector3.Dot(candidateDirection.normalized,
                normalizedDesiredDirection);
            if (alignment <= bestAlignment)
            {
                continue;
            }
            bestAlignment = alignment;
            bestDirection = candidateDirection.normalized;
        }
        // Permit reversal when no other direction is available.
        if(bestDirection.sqrMagnitude <= 0f)
        {
            foreach (Vector3 canadidateDirection in context.AvailableDirections)
            {
                float alignment = Vector3.Dot(canadidateDirection.normalized,
                    normalizedDesiredDirection);
                if (alignment <= bestAlignment)
                {
                    continue;
                }
                bestAlignment = alignment;
                bestDirection = canadidateDirection.normalized;
            }
        }
        // Commit to the selected direction.
        context.SetCommittedDirection(bestDirection);
        return bestDirection;
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
