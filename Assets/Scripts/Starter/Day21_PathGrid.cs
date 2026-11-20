using System.Collections.Generic;
using UnityEngine;

public class Day21_PathGrid : MonoBehaviour
{
    #region Inspector Variables

    [SerializeField]
    private int gridWidth = 9;

    [SerializeField]
    private int gridHeight = 9;

    [SerializeField]
    private float nodeSize = 1f;

    [SerializeField]
    private Transform gridOrigin;

    [SerializeField]
    private Transform humanTransform;

    [SerializeField]
    private Transform chestTransform;

    [SerializeField]
    private LayerMask obstacleMask;

    [SerializeField]
    private List<Day21_GridNode> nodes = new List<Day21_GridNode>();

    #endregion

    #region Private Variables

    private List<Day21_GridNode> currentPath = new List<Day21_GridNode>();

    #endregion

    #region Public Properties

    public Day21_GridNode StartNode
    {
        get
        {
            if (humanTransform == null)
            {
                return null;
            }

            return WorldToNode(humanTransform.position);
        }
    }

    public Day21_GridNode GoalNode
    {
        get
        {
            if (chestTransform == null)
            {
                return null;
            }

            return WorldToNode(chestTransform.position);
        }
    }

    public List<Day21_GridNode> CurrentPath
    {
        get
        {
            return currentPath;
        }
    }

    #endregion

    #region Unity Methods

    /// <summary>
    /// Refreshes walkability when Play mode starts.
    /// </summary>
    public void Awake()
    {
        RefreshWalkability();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Converts a grid coordinate into the world-space centre of that cell.
    /// </summary>
    /// <param name="gridX">The horizontal grid coordinate.</param>
    /// <param name="gridY">The vertical grid coordinate.</param>
    /// <returns>The world-space centre of the grid cell.</returns>
    public Vector3 GridToWorld(int gridX, int gridY)
    {
        // Calculate the X/Z offset from the grid origin.
        Vector3 gridOffset = new Vector3(gridX * nodeSize, 0f, gridY * nodeSize);

        // Calculate the half-cell centre offset.
        Vector3 centreOffset = new Vector3(nodeSize * 0.5f, 0f, nodeSize * 0.5f);

        // Combine origin, offset, and centre offset.
        return gridOrigin.position + gridOffset + centreOffset;
    }

    /// <summary>
    /// Converts a Unity world position into a node from the authored grid.
    /// </summary>
    /// <param name="worldPosition">The Unity world position to locate.</param>
    /// <returns>The matching node, or null when the position is outside the grid.</returns>
    public Day21_GridNode WorldToNode(Vector3 worldPosition)
    {
        // Express the world position relative to the grid origin.
        Vector3 relativePosition = worldPosition - gridOrigin.position;

        // Convert the relative X/Z position into grid coordinates.
        int gridX = Mathf.FloorToInt(relativePosition.x / nodeSize);
        int gridY = Mathf.FloorToInt(relativePosition.z / nodeSize);

        // Reject coordinates outside the grid.
        if (gridX < 0 || gridX >= gridWidth || gridY < 0 || gridY >= gridHeight)
        {
            return null;
        }

        // Return the node at the valid coordinate.
        return GetNode(gridX, gridY);
    }

    /// <summary>
    /// Updates walkability for every authored node using the current obstacle geometry.
    /// </summary>
    [ContextMenu("Refresh Walkability")]
    public void RefreshWalkability()
    {
        // Calculate the CheckBox half extents once.
        Vector3 halfExtents = new Vector3(nodeSize * 0.45f, 0.5f, nodeSize * 0.45f);

        // Evaluate every authored node against the Obstacle layer.
        foreach(Day21_GridNode node in nodes)
        {
            if (node == null)
            {
                continue;
            }
            bool isBlocked = Physics.CheckBox(node.WorldPosition, halfExtents, Quaternion.identity, 
                obstacleMask, QueryTriggerInteraction.Ignore);
            node.IsWalkable = !isBlocked;
        }

    }

    /// <summary>
    /// Returns the walkable cardinal neighbours of a node.
    /// </summary>
    /// <param name="node">The node whose neighbours are requested.</param>
    /// <returns>The walkable neighbouring nodes.</returns>
    public List<Day21_GridNode> GetNeighbours(Day21_GridNode node)
    {
        List<Day21_GridNode> neighbours = new List<Day21_GridNode>();
        Vector2Int coordinate = node.Coordinate;
        AddNeighbourIfWalkable(neighbours, coordinate.x, coordinate.y + 1);
        AddNeighbourIfWalkable(neighbours, coordinate.x + 1, coordinate.y);
        AddNeighbourIfWalkable(neighbours, coordinate.x, coordinate.y - 1);
        AddNeighbourIfWalkable(neighbours, coordinate.x - 1, coordinate.y);
        return neighbours;
    }

    /// <summary>
    /// Logs every authored node whose Transform does not match GridToWorld.
    /// </summary>
    [ContextMenu("Check Grid To World")]
    public void CheckGridToWorld()
    {
        int mismatches = 0;

        foreach (Day21_GridNode node in nodes)
        {
            Vector3 expected = GridToWorld(node.Coordinate.x, node.Coordinate.y);

            if (Vector3.Distance(expected, node.WorldPosition) > 0.01f)
            {
                mismatches++;
                Debug.LogWarning("Node " + node.Coordinate + " expected " + expected + " but is at " + node.WorldPosition, node);
            }
        }

        Debug.Log(mismatches == 0 ? "GridToWorld matches all " + nodes.Count + " nodes." : mismatches + " node(s) do not match GridToWorld.");
    }

    /// <summary>
    /// Clears A* state from every node before another search.
    /// </summary>
    public void ResetSearch()
    {
        foreach (Day21_GridNode node in nodes)
        {
            node.ResetSearch();
        }

        currentPath.Clear();
    }

    /// <summary>
    /// Stores the latest path for visualization.
    /// </summary>
    /// <param name="path">The path produced by A*.</param>
    public void SetPath(List<Day21_GridNode> path)
    {
        currentPath = path;
    }

    /// <summary>
    /// Runs A* using the Human's current node and the Chest's current node.
    /// </summary>
    [ContextMenu("Run Search")]
    public void RunSearch()
    {
        Day21_AStarPathfinder pathfinder = new Day21_AStarPathfinder(this);
        SetPath(pathfinder.FindPath(StartNode, GoalNode));
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Returns the authored node at a grid coordinate.
    /// </summary>
    /// <param name="gridX">The horizontal grid coordinate.</param>
    /// <param name="gridY">The vertical grid coordinate.</param>
    /// <returns>The matching node, or null for an invalid coordinate.</returns>
    private Day21_GridNode GetNode(int gridX, int gridY)
    {
        if (gridX < 0 || gridX >= gridWidth || gridY < 0 || gridY >= gridHeight)
        {
            return null;
        }

        int nodeIndex = gridY * gridWidth + gridX;

        if (nodeIndex < 0 || nodeIndex >= nodes.Count)
        {
            return null;
        }

        return nodes[nodeIndex];
    }

    /// <summary>
    /// Adds a node to the neighbour collection when it exists and is walkable.
    /// </summary>
    private void AddNeighbourIfWalkable(List<Day21_GridNode> neighbours, int gridX, int gridY)
    {
        Day21_GridNode neighbourNode = GetNode(gridX, gridY);

        if (neighbourNode == null || !neighbourNode.IsWalkable)
        {
            return;
        }

        neighbours.Add(neighbourNode);
    }

    /// <summary>
    /// Draws the authored grid and current A* state over the dungeon environment.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (nodes == null)
        {
            return;
        }

        foreach (Day21_GridNode node in nodes)
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
            Gizmos.DrawCube(node.WorldPosition, new Vector3(nodeSize * 0.9f, 3.0f, nodeSize * 0.9f));
        }
    }

    #endregion
}
