using UnityEngine;

/// <summary>
/// Stores search data for one resolved Day 20 grid cell.
/// </summary>
public class Day20_GridNode
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
    public Day20_GridNode Parent { get; set; }
    #endregion

    #region Constructors
    public Day20_GridNode(Vector2Int coordinate, Vector3 worldPosition, bool isWalkable)
    {
        Coordinate = coordinate;
        WorldPosition = worldPosition;
        IsWalkable = isWalkable;
        ResetSearch();
    }
    #endregion

    #region Public Methods
    public void CalculateFCost()
    {
        FCost = GCost + HCost;
    }

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
