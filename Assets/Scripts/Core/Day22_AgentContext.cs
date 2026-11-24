using System.Collections.Generic;
using UnityEngine;

public class Day22_AgentContext : MonoBehaviour
{
    #region Public Variables

    public bool HasCommittedDirection;
    public Vector3 CurrentDirection;
    public List<Vector3> AvailableDirections = new List<Vector3>();
    public List<Day22_GridNode> CurrentPath = new List<Day22_GridNode>();
    public int CurrentPathIndex;
    public Day22_GridNode PreviousGoalNode;

    #endregion

    #region Public Methods

    /// <summary>
    /// Removes local movement directions from the previous sensor update.
    /// </summary>
    public void ClearAvailableDirections()
    {
        AvailableDirections.Clear();
    }

    /// <summary>
    /// Adds a normalized local movement direction.
    /// </summary>
    public void AddAvailableDirection(Vector3 direction)
    {
        Vector3 normalizedDirection = direction.normalized;

        if (normalizedDirection.sqrMagnitude <= 0f)
        {
            return;
        }

        AvailableDirections.Add(normalizedDirection);
    }

    /// <summary>
    /// Stores the direction the agent should continue using while useful.
    /// </summary>
    public void SetCommittedDirection(Vector3 direction)
    {
        CurrentDirection = direction.normalized;
        HasCommittedDirection = CurrentDirection.sqrMagnitude > 0f;
    }

    /// <summary>
    /// Clears the currently committed direction.
    /// </summary>
    public void ClearCommittedDirection()
    {
        CurrentDirection = Vector3.zero;
        HasCommittedDirection = false;
    }

    /// <summary>
    /// Stores a newly calculated A* path and resets path following.
    /// </summary>
    public void SetPath(List<Day22_GridNode> path, Day22_GridNode goalNode)
    {
        CurrentPath = path;
        CurrentPathIndex = CurrentPath.Count > 1 ? 1 : 0;
        PreviousGoalNode = goalNode;
        ClearCommittedDirection();
    }

    #endregion
}
