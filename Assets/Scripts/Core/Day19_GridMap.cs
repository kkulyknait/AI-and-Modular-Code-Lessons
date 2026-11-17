using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Builds a small searchable grid for the Day 19 A* demonstration.
/// This support class is supplied complete so class time stays focused on A*.
/// </summary>
public class Day19_GridMap : MonoBehaviour
{
    #region Inspector Variables
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 7;
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private Vector2Int startCoordinate = new Vector2Int(1, 1);
    [SerializeField] private Vector2Int goalCoordinate = new Vector2Int(8, 5);
    [SerializeField] private List<Vector2Int> blockedCoordinates = new List<Vector2Int>();
    #endregion

    #region Private Variables
    private Day19_GridNode[,] nodes;
    private List<Day19_GridNode> currentPath = new List<Day19_GridNode>();
    #endregion

    #region Public Properties
    public Day19_GridNode StartNode => GetNode(startCoordinate);
    public Day19_GridNode GoalNode => GetNode(goalCoordinate);
    #endregion

    #region Unity Methods
    /// <summary>
    /// Builds the grid when the scene begins.
    /// </summary>
    private void Awake()
    {
        BuildGrid();
    }

    /// <summary>
    /// Draws the searchable grid, blocked cells, start, goal, and final path.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (nodes == null)
        {
            return;
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Day19_GridNode node = nodes[x, y];
                Gizmos.color = node.IsWalkable ? Color.white : Color.black;

                if (node == StartNode)
                {
                    Gizmos.color = Color.green;
                }
                else if (node == GoalNode)
                {
                    Gizmos.color = Color.red;
                }
                else if (currentPath.Contains(node))
                {
                    Gizmos.color = Color.cyan;
                }
                else if (node.IsClosed)
                {
                    Gizmos.color = new Color(1f, 0.55f, 0f);
                }
                else if (node.IsOpen)
                {
                    Gizmos.color = Color.yellow;
                }

                Gizmos.DrawCube(node.WorldPosition, Vector3.one * cellSize * 0.8f);
            }
        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Returns the walkable orthogonal neighbours of a node.
    /// </summary>
    /// <param name="node">The node whose connections are requested.</param>
    /// <returns>The valid neighbouring nodes.</returns>
    public List<Day19_GridNode> GetNeighbours(Day19_GridNode node)
    {
        List<Day19_GridNode> neighbours = new List<Day19_GridNode>();
        // Order: Up, Right, Down, Left.
        AddNeighbour(neighbours, node.Coordinate + Vector2Int.up);
        AddNeighbour(neighbours, node.Coordinate + Vector2Int.right);
        AddNeighbour(neighbours, node.Coordinate + Vector2Int.down);
        AddNeighbour(neighbours, node.Coordinate + Vector2Int.left);
        return neighbours;
    }

    /// <summary>
    /// Clears old search values from every node.
    /// </summary>
    public void ResetSearch()
    {
        currentPath.Clear();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                nodes[x, y].ResetSearch();
            }
        }
    }

    /// <summary>
    /// Stores the result so it can be drawn in the Scene view.
    /// </summary>
    /// <param name="path">The reconstructed start-to-goal path.</param>
    public void SetPath(List<Day19_GridNode> path)
    {
        currentPath = path;
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Creates the node data used by the search.
    /// </summary>
    private void BuildGrid()
    {
        nodes = new Day19_GridNode[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int coordinate = new Vector2Int(x, y);
                Vector3 worldPosition = transform.position + new Vector3(x * cellSize, 0f, y * cellSize);
                bool isWalkable = !blockedCoordinates.Contains(coordinate);
                nodes[x, y] = new Day19_GridNode(coordinate, worldPosition, isWalkable);
            }
        }
    }

    /// <summary>
    /// Adds a valid walkable coordinate to the neighbour collection.
    /// </summary>
    private void AddNeighbour(List<Day19_GridNode> neighbours, Vector2Int coordinate)
    {
        Day19_GridNode node = GetNode(coordinate);

        if (node != null && node.IsWalkable)
        {
            neighbours.Add(node);
        }
    }

    /// <summary>
    /// Gets one node, returning null for coordinates outside the grid.
    /// </summary>
    private Day19_GridNode GetNode(Vector2Int coordinate)
    {
        if (nodes == null || coordinate.x < 0 || coordinate.x >= width || coordinate.y < 0 || coordinate.y >= height)
        {
            return null;
        }

        return nodes[coordinate.x, coordinate.y];
    }
    #endregion
}