using UnityEngine;

public class Day21_GridNode : MonoBehaviour
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
    public Day21_GridNode Parent;

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

    public void Awake()
    {
        coordinate.x = Mathf.RoundToInt(transform.position.x);
        coordinate.y = Mathf.RoundToInt(transform.position.z);
    }

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
