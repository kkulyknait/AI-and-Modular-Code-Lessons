using UnityEngine;

public class Day22_GridNode : MonoBehaviour
{
    #region Inspector Variables

    [SerializeField]
    private Vector2Int coordinate;

    [SerializeField]
    private bool isWalkable = true;

    #endregion

    #region Public Variables

    public int GCost;
    public int HCost;
    public int FCost;
    public bool IsOpen;
    public bool IsClosed;
    public Day22_GridNode Parent;

    #endregion

    #region Public Properties

    public Vector2Int Coordinate
    {
        get
        {
            return coordinate;
        }
    }

    public Vector3 WorldPosition
    {
        get
        {
            return transform.position;
        }
    }

    public bool IsWalkable
    {
        get
        {
            return isWalkable;
        }
        set
        {
            isWalkable = value;
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Assigns the grid coordinate calculated by Day22_PathGrid.
    /// </summary>
    /// <param name="newCoordinate">The coordinate represented by this node.</param>
    public void SetCoordinate(Vector2Int newCoordinate)
    {
        coordinate = newCoordinate;
    }

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
