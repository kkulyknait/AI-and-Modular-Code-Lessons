using System.Collections.Generic;
using UnityEngine;

public class IntermediateAgentContext : MonoBehaviour
{
    #region Inspector Variables

    [SerializeField]
    private List<Transform> knownCoinLocations = new List<Transform>();

    #endregion

    #region Public Variables

    public bool HasKey;
    public Transform VisibleKey;
    public Transform VisibleCoin;
    public Transform VisibleChest;
    public Transform ResponseTarget;
    public bool HasPersistentTarget;
    public Transform PersistentTarget;
    public bool HasCommittedDirection;
    public Vector3 CurrentDirection;
    public List<Vector3> AvailableDirections = new List<Vector3>();
    public bool HasSearchTarget;
    public Vector3 SearchTargetPosition;
    public int CurrentSearchIndex = -1;

    #endregion

    #region Public Properties

    public List<Transform> KnownCoinLocations
    {
        get
        {
            return knownCoinLocations;
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Clears the objects perceived during the previous perception update.
    /// </summary>
    public void ClearPerception()
    {
        VisibleKey = null;
        VisibleCoin = null;
        VisibleChest = null;
    }

    /// <summary>
    /// Removes the direction results from the previous availability evaluation.
    /// </summary>
    public void ClearAvailableDirections()
    {
        AvailableDirections.Clear();
    }

    /// <summary>
    /// Adds a normalized movement direction to the available-direction list.
    /// </summary>
    /// <param name="direction">The available world-space direction.</param>
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
    /// Stores a target that should continue influencing the agent across frames.
    /// </summary>
    /// <param name="target">The target the agent should remember.</param>
    public void SetPersistentTarget(Transform target)
    {
        PersistentTarget = target;
        HasPersistentTarget = target != null;
    }

    /// <summary>
    /// Clears the currently remembered persistent target.
    /// </summary>
    public void ClearPersistentTarget()
    {
        PersistentTarget = null;
        HasPersistentTarget = false;
    }

    /// <summary>
    /// Stores the movement direction the agent should continue using.
    /// </summary>
    /// <param name="direction">The selected movement direction.</param>
    public void SetCommittedDirection(Vector3 direction)
    {
        CurrentDirection = direction.normalized;
        HasCommittedDirection = CurrentDirection.sqrMagnitude > 0f;
    }

    /// <summary>
    /// Clears the currently committed movement direction.
    /// </summary>
    public void ClearCommittedDirection()
    {
        CurrentDirection = Vector3.zero;
        HasCommittedDirection = false;
    }

    /// <summary>
    /// Stores a remembered world-space search position.
    /// </summary>
    /// <param name="position">The position the agent should search.</param>
    public void SetSearchTarget(Vector3 position)
    {
        SearchTargetPosition = position;
        HasSearchTarget = true;
    }

    /// <summary>
    /// Clears the current remembered search position.
    /// </summary>
    public void ClearSearchTarget()
    {
        SearchTargetPosition = Vector3.zero;
        HasSearchTarget = false;
    }

    /// <summary>
    /// Advances to the next configured Coin location.
    /// </summary>
    /// <returns>True when a valid location was selected.</returns>
    public bool SelectNextCoinLocation()
    {
        if (knownCoinLocations.Count == 0)
        {
            ClearSearchTarget();
            return false;
        }

        int checkedLocations = 0;

        while (checkedLocations < knownCoinLocations.Count)
        {
            CurrentSearchIndex++;

            if (CurrentSearchIndex >= knownCoinLocations.Count)
            {
                CurrentSearchIndex = 0;
            }

            Transform coinLocation = knownCoinLocations[CurrentSearchIndex];
            checkedLocations++;

            if (coinLocation == null)
            {
                continue;
            }

            SetSearchTarget(coinLocation.position);
            return true;
        }

        ClearSearchTarget();
        return false;
    }

    #endregion
}
