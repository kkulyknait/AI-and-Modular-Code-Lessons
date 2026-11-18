using UnityEngine;

/// <summary>
/// Stores location, walkability, costs, list state, and parent data for one searchable cell.
/// </summary>
public class Day19_GridNode
{
    #region Public Properties
    public Vector2Int Coordinate { get; private set; }
    public Vector3 WorldPosition { get; private set; }
    public bool IsWalkable { get; private set; }
    public int GCost { get; set; }
    public int HCost { get; set; }
    public int FCost { get; private set; }
    public bool IsOpen { get; set; }
    public bool IsClosed { get; set; }
    public Day19_GridNode Parent { get; set; }
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a node at a grid coordinate.
    /// </summary>
    public Day19_GridNode(Vector2Int coordinate, Vector3 worldPosition, bool isWalkable)
    {
        Coordinate = coordinate;
        WorldPosition = worldPosition;
        IsWalkable = isWalkable;
        ResetSearch();
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Updates the total estimated path cost.
    /// </summary>
    public void CalculateFCost()
    {
        FCost = GCost + HCost;
    }

    /// <summary>
    /// Restores the node to an unused search state.
    /// </summary>
    public void ResetSearch()
    {
        GCost = int.MaxValue;
        HCost = 0;
        FCost = int.MaxValue;
        IsOpen = false;
        IsClosed = false;
        Parent = null;
    }
    #endregion
}