using System.Collections.Generic;
using UnityEngine;

public class Day22_PathGrid : MonoBehaviour
{
    #region Inspector Variables

    [SerializeField]
    private int gridWidth = 9;

    [SerializeField]
    private int gridHeight = 9;

    [SerializeField]
    private float nodeSize = 1f;

    [SerializeField]
    private float gizmoHeight = 0.3f;

    [SerializeField]
    private Transform gridOrigin;

    [SerializeField]
    private Transform humanTransform;

    [SerializeField]
    private Transform chestTransform;

    [SerializeField]
    private LayerMask obstacleMask;

    [SerializeField]
    private Day22_AStarPathfinder pathfinder;

    [SerializeField]
    private List<Day22_GridNode> nodes = new List<Day22_GridNode>();

    #endregion

    #region Private Variables

    private List<Day22_GridNode> currentPath = new List<Day22_GridNode>();

    #endregion

    #region Public Properties

    public Day22_GridNode StartNode
    {
        get
        {
            return humanTransform == null ? null : WorldToNode(humanTransform.position);
        }
    }

    public Day22_GridNode GoalNode
    {
        get
        {
            return chestTransform == null ? null : WorldToNode(chestTransform.position);
        }
    }

    public List<Day22_GridNode> CurrentPath
    {
        get
        {
            return currentPath;
        }
    }

    #endregion

    #region Unity Methods

    /// <summary>
    /// Prepares node coordinates and static walkability before the first path request.
    /// </summary>
    private void Awake()
    {
        ConfigureNodeCoordinates();
        RefreshWalkability();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Converts a world position into a grid coordinate using the configured origin and node size.
    /// </summary>
    public Vector2Int WorldToCoordinate(Vector3 worldPosition)
    {
        Vector3 relativePosition = worldPosition - gridOrigin.position;
        int gridX = Mathf.FloorToInt(relativePosition.x / nodeSize);
        int gridY = Mathf.FloorToInt(relativePosition.z / nodeSize);
        return new Vector2Int(gridX, gridY);
    }

    /// <summary>
    /// Converts a Unity world position into a node from the authored grid.
    /// </summary>
    public Day22_GridNode WorldToNode(Vector3 worldPosition)
    {
        Vector2Int coordinate = WorldToCoordinate(worldPosition);

        if (coordinate.x < 0 || coordinate.x >= gridWidth || coordinate.y < 0 || coordinate.y >= gridHeight)
        {
            return null;
        }

        return GetNode(coordinate.x, coordinate.y);
    }

    /// <summary>
    /// Updates walkability using only permanent A* obstacles.
    /// </summary>
    public void RefreshWalkability()
    {
        Vector3 halfExtents = new Vector3(nodeSize * 0.45f, 0.5f, nodeSize * 0.45f);

        foreach (Day22_GridNode node in nodes)
        {
            if (node == null)
            {
                continue;
            }

            bool isBlocked = Physics.CheckBox(node.WorldPosition, halfExtents, Quaternion.identity, obstacleMask, QueryTriggerInteraction.Ignore);
            node.IsWalkable = !isBlocked;
        }
    }

    /// <summary>
    /// Returns the walkable cardinal neighbours of a node in Up, Right, Down, Left order.
    /// </summary>
    public List<Day22_GridNode> GetNeighbours(Day22_GridNode node)
    {
        List<Day22_GridNode> neighbours = new List<Day22_GridNode>();
        Vector2Int coordinate = node.Coordinate;
        AddNeighbourIfWalkable(neighbours, coordinate.x, coordinate.y + 1);
        AddNeighbourIfWalkable(neighbours, coordinate.x + 1, coordinate.y);
        AddNeighbourIfWalkable(neighbours, coordinate.x, coordinate.y - 1);
        AddNeighbourIfWalkable(neighbours, coordinate.x - 1, coordinate.y);
        return neighbours;
    }

    /// <summary>
    /// Clears A* state before another search.
    /// </summary>
    public void ResetSearch()
    {
        foreach (Day22_GridNode node in nodes)
        {
            node.ResetSearch();
        }

        currentPath.Clear();
    }

    /// <summary>
    /// Runs A* from the Human to the Chest and returns the generated path.
    /// </summary>
    public List<Day22_GridNode> RunSearch()
    {
        if (pathfinder == null)
        {
            currentPath.Clear();
            return currentPath;
        }

        currentPath = pathfinder.FindPath(this, StartNode, GoalNode);
        return currentPath;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Assigns coordinates using the same world-to-grid convention used for the Human and Chest.
    /// </summary>
    private void ConfigureNodeCoordinates()
    {
        foreach (Day22_GridNode node in nodes)
        {
            if (node == null)
            {
                continue;
            }

            node.SetCoordinate(WorldToCoordinate(node.WorldPosition));
        }
    }

    private Day22_GridNode GetNode(int gridX, int gridY)
    {
        if (gridX < 0 || gridX >= gridWidth || gridY < 0 || gridY >= gridHeight)
        {
            return null;
        }

        int nodeIndex = gridY * gridWidth + gridX;
        return nodeIndex < 0 || nodeIndex >= nodes.Count ? null : nodes[nodeIndex];
    }

    private void AddNeighbourIfWalkable(List<Day22_GridNode> neighbours, int gridX, int gridY)
    {
        Day22_GridNode neighbourNode = GetNode(gridX, gridY);

        if (neighbourNode == null || !neighbourNode.IsWalkable)
        {
            return;
        }

        neighbours.Add(neighbourNode);
    }

    private void OnDrawGizmos()
    {
        if (nodes == null)
        {
            return;
        }

        foreach (Day22_GridNode node in nodes)
        {
            if (node == null)
            {
                continue;
            }

            Color nodeColor = node.IsWalkable ? new Color(1f, 1f, 1f, 0.12f) : new Color(0f, 0f, 0f, 0.45f);

            if (node == StartNode)
            {
                nodeColor = new Color(0f, 1f, 0f, 0.55f);
            }
            else if (node == GoalNode)
            {
                nodeColor = new Color(1f, 0f, 0f, 0.55f);
            }
            else if (currentPath != null && currentPath.Contains(node))
            {
                nodeColor = new Color(0f, 1f, 1f, 0.45f);
            }
            else if (node.IsClosed)
            {
                nodeColor = new Color(1f, 0.55f, 0f, 0.20f);
            }
            else if (node.IsOpen)
            {
                nodeColor = new Color(1f, 1f, 0f, 0.20f);
            }

            Gizmos.color = nodeColor;
            Vector3 gizmoPosition = node.WorldPosition + Vector3.up * gizmoHeight;
            Gizmos.DrawCube(gizmoPosition, new Vector3(nodeSize * 0.9f, 0.05f, nodeSize * 0.9f));
        }
    }

    #endregion
}
