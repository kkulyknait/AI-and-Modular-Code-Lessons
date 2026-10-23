using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(IntermediateAgentContext))]
public abstract class IntermediateAgentController : MonoBehaviour
{
    #region Inspector Variables

    [SerializeField]
    private float moveSpeed = 3f;

    [SerializeField]
    private float turnSpeed = 8f;

    [SerializeField]
    private float stopDistance = 0.75f;

    #endregion

    #region Protected Variables

    protected IntermediateAgentContext context;
    protected Rigidbody rigidBody;

    #endregion

    #region Unity Methods

    /// <summary>
    /// Gets the required components and configures top-down Rigidbody movement.
    /// </summary>
    protected virtual void Awake()
    {
        context = GetComponent<IntermediateAgentContext>();
        rigidBody = GetComponent<Rigidbody>();

        rigidBody.useGravity = false;
        rigidBody.constraints = RigidbodyConstraints.FreezePositionY
            | RigidbodyConstraints.FreezeRotationX
            | RigidbodyConstraints.FreezeRotationZ;
    }

    /// <summary>
    /// Selects and executes the agent's movement during the physics update.
    /// </summary>
    private void FixedUpdate()
    {
        context.ResponseTarget = null;

        Vector3 desiredDirection = GetDesiredDirection();
        desiredDirection.y = 0f;

        if (desiredDirection.sqrMagnitude <= 0f)
        {
            desiredDirection = GetWanderDirection();
        }

        Vector3 movementDirection = SelectBestAvailableDirection(desiredDirection, false);

        if (movementDirection.sqrMagnitude <= 0f)
        {
            return;
        }

        RotateAgent(movementDirection);
        MoveAgent(movementDirection);
    }

    #endregion

    #region Protected Methods

    /// <summary>
    /// Returns the movement direction requested by the derived agent.
    /// </summary>
    /// <returns>The desired movement direction.</returns>
    protected abstract Vector3 GetDesiredDirection();

    /// <summary>
    /// Returns a horizontal direction toward a Transform.
    /// </summary>
    /// <param name="target">The target Transform.</param>
    /// <returns>A horizontal direction toward the target.</returns>
    protected Vector3 GetDirectionTo(Transform target)
    {
        if (target == null)
        {
            return Vector3.zero;
        }

        return GetDirectionTo(target.position);
    }

    /// <summary>
    /// Returns a horizontal direction toward a world-space position.
    /// </summary>
    /// <param name="targetPosition">The target world-space position.</param>
    /// <returns>A horizontal direction toward the position.</returns>
    protected Vector3 GetDirectionTo(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - rigidBody.position;
        direction.y = 0f;
        return direction;
    }

    /// <summary>
    /// Stores the current response target for inspection.
    /// </summary>
    /// <param name="target">The active response target.</param>
    protected void SetResponseTarget(Transform target)
    {
        context.ResponseTarget = target;
    }

    /// <summary>
    /// Ensures the agent has a configured Coin search target.
    /// </summary>
    protected void EnsureCoinSearchTarget()
    {
        if (!context.HasSearchTarget)
        {
            context.SelectNextCoinLocation();
        }
    }

    /// <summary>
    /// Advances the Coin search after the current location is reached.
    /// </summary>
    protected void AdvanceCoinSearchIfRequired()
    {
        if (!context.HasSearchTarget)
        {
            return;
        }

        if (GetDirectionTo(context.SearchTargetPosition).magnitude > stopDistance)
        {
            return;
        }

        context.ClearSearchTarget();
        context.SelectNextCoinLocation();
        context.ClearCommittedDirection();
    }

    /// <summary>
    /// Selects the available direction that best supports the desired direction.
    /// </summary>
    /// <param name="desiredDirection">The direction requested by the active behaviour.</param>
    /// <param name="canReverse">Whether immediate reversal is allowed.</param>
    /// <returns>The selected available direction.</returns>
    protected Vector3 SelectBestAvailableDirection(Vector3 desiredDirection, bool canReverse)
    {
        // Handle the case where no directions are available.

        // Normalize the desired direction before comparing alignments.

        // Keep the committed direction while it remains available and useful.

        // Track the best normal candidate.

        // Compare each direction with the desired direction.

        // Permit reversal when no normal candidate is available.

        // Commit to the selected direction.
        return Vector3.zero;
    }

    /// <summary>
    /// Returns a persistent direction for default wandering.
    /// </summary>
    /// <returns>The selected wander direction.</returns>
    protected Vector3 GetWanderDirection()
    {
        // Keep the committed wander direction while it remains available.

        // Stop when no directions are available.

        // Build a list that excludes immediate reversal.

        // Permit reversal when the agent reaches a dead end.

        // Select and commit a random direction.
        return Vector3.zero;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Rotates the Rigidbody toward a movement direction.
    /// </summary>
    /// <param name="movementDirection">The selected movement direction.</param>
    private void RotateAgent(Vector3 movementDirection)
    {
        Quaternion targetRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
        Quaternion nextRotation = Quaternion.Slerp(rigidBody.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);
        rigidBody.MoveRotation(nextRotation);
    }

    /// <summary>
    /// Moves the Rigidbody in a selected direction.
    /// </summary>
    /// <param name="movementDirection">The selected movement direction.</param>
    private void MoveAgent(Vector3 movementDirection)
    {
        Vector3 nextPosition = rigidBody.position + movementDirection * moveSpeed * Time.fixedDeltaTime;
        rigidBody.MovePosition(nextPosition);
    }

    /// <summary>
    /// Determines whether a direction is currently available.
    /// </summary>
    /// <param name="direction">The direction being tested.</param>
    /// <returns>True when the direction is available.</returns>
    private bool IsDirectionAvailable(Vector3 direction)
    {
        foreach (Vector3 availableDirection in context.AvailableDirections)
        {
            float alignment = Vector3.Dot(availableDirection.normalized, direction.normalized);

            if (alignment > 0.99f)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Determines whether a candidate reverses the committed direction.
    /// </summary>
    /// <param name="candidateDirection">The candidate direction.</param>
    /// <returns>True when the candidate is approximately opposite.</returns>
    private bool IsReverseDirection(Vector3 candidateDirection)
    {
        if (!context.HasCommittedDirection)
        {
            return false;
        }

        float alignment = Vector3.Dot(context.CurrentDirection.normalized, candidateDirection.normalized);
        return alignment < -0.9f;
    }

    #endregion
}
