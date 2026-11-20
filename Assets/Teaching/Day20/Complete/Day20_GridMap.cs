using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Converts a fixed source grid into a higher-resolution searchable grid.
/// Each source cell subdivides uniformly and preserves its walkability.
/// </summary>
public class Day20_GridMap : MonoBehaviour
{
    #region Inspector Variables
    [Header("Source Grid")]
    [SerializeField] private int baseWidth = 10;
    [SerializeField] private int baseHeight = 7;
    [SerializeField] private float baseCellSize = 1f;
    [SerializeField] private Vector2Int baseStartCoordinate = new Vector2Int(1, 1);
    [SerializeField] private Vector2Int baseGoalCoordinate = new Vector2Int(8, 5);
    [SerializeField] private List<Vector2Int> blockedSourceCoordinates = new List<Vector2Int>();

    [Header("Resolution")]
    [SerializeField, Min(1)] private int startingResolution = 1;
    [SerializeField, Min(1)] private int maximumResolution = 8;
    #endregion

    #region Private Variables
    private int resolution;
    private Day20_GridNode[,] nodes;
    private List<Day20_GridNode> currentPath = new List<Day20_GridNode>();
    #endregion

    #region Public Properties
    public int Resolution => resolution;
    public int ResolvedWidth => baseWidth * resolution;
    public int ResolvedHeight => baseHeight * resolution;
    public float ResolvedCellSize => baseCellSize / resolution;
    public int TotalNodeCount => ResolvedWidth * ResolvedHeight;
    public int WalkableNodeCount { get; private set; }
    public Day20_GridNode StartNode => GetNode(BaseToResolvedCoordinate(baseStartCoordinate));
    public Day20_GridNode GoalNode => GetNode(BaseToResolvedCoordinate(baseGoalCoordinate));
    public bool CanIncreaseResolution => resolution < maximumResolution;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        resolution = Mathf.Max(1, startingResolution);
        BuildGrid();
    }

    private void OnDrawGizmos()
    {
        if (nodes == null)
        {
            return;
        }

        float resolvedCellSize = ResolvedCellSize;
        for (int x = 0; x < ResolvedWidth; x++)
        {
            for (int y = 0; y < ResolvedHeight; y++)
            {
                Day20_GridNode node = nodes[x, y];
                Gizmos.color = node.IsWalkable ? Color.white : Color.black;

                if (node == StartNode) Gizmos.color = Color.green;
                else if (node == GoalNode) Gizmos.color = Color.red;
                else if (currentPath.Contains(node)) Gizmos.color = Color.cyan;
                else if (node.IsClosed) Gizmos.color = new Color(1f, 0.55f, 0f);
                else if (node.IsOpen) Gizmos.color = Color.yellow;

                Gizmos.DrawCube(node.WorldPosition, Vector3.one * resolvedCellSize * 0.8f);
            }
        }
    }
    #endregion

    #region Public Methods
    public List<Day20_GridNode> GetNeighbours(Day20_GridNode node)
    {
        List<Day20_GridNode> neighbours = new List<Day20_GridNode>();
        // Same order as Day 19: Up, Right, Down, Left.
        AddNeighbour(neighbours, node.Coordinate + Vector2Int.up);
        AddNeighbour(neighbours, node.Coordinate + Vector2Int.right);
        AddNeighbour(neighbours, node.Coordinate + Vector2Int.down);
        AddNeighbour(neighbours, node.Coordinate + Vector2Int.left);
        return neighbours;
    }

    public bool IncreaseResolution()
    {
        if (!CanIncreaseResolution)
        {
            return false;
        }

        resolution = Mathf.Min(resolution * 2, maximumResolution);
        BuildGrid();
        return true;
    }

    public void ResetSearch()
    {
        currentPath.Clear();
        for (int x = 0; x < ResolvedWidth; x++)
        {
            for (int y = 0; y < ResolvedHeight; y++)
            {
                nodes[x, y].ResetSearch();
            }
        }
    }

    public void SetPath(List<Day20_GridNode> path)
    {
        currentPath = path;
    }
    #endregion

    #region Private Methods
    private void BuildGrid()
    {
        nodes = new Day20_GridNode[ResolvedWidth, ResolvedHeight];
        currentPath.Clear();
        WalkableNodeCount = 0;
        float resolvedCellSize = ResolvedCellSize;

        for (int x = 0; x < ResolvedWidth; x++)
        {
            for (int y = 0; y < ResolvedHeight; y++)
            {
                Vector2Int coordinate = new Vector2Int(x, y);
                Vector2Int sourceCoordinate = new Vector2Int(x / resolution, y / resolution);
                bool isWalkable = !blockedSourceCoordinates.Contains(sourceCoordinate);
                // Node positions are the centre of each cell.
                Vector3 worldPosition = transform.position + new Vector3((x + 0.5f) * resolvedCellSize, 0f, (y + 0.5f) * resolvedCellSize);
                nodes[x, y] = new Day20_GridNode(coordinate, worldPosition, isWalkable);

                if (isWalkable)
                {
                    WalkableNodeCount++;
                }
            }
        }
    }

    private Vector2Int BaseToResolvedCoordinate(Vector2Int sourceCoordinate)
    {
        // The lowest-left child represents the source coordinate at higher resolutions.
        return sourceCoordinate * resolution;
    }

    private void AddNeighbour(List<Day20_GridNode> neighbours, Vector2Int coordinate)
    {
        Day20_GridNode node = GetNode(coordinate);
        if (node != null && node.IsWalkable)
        {
            neighbours.Add(node);
        }
    }

    private Day20_GridNode GetNode(Vector2Int coordinate)
    {
        if (nodes == null || coordinate.x < 0 || coordinate.x >= ResolvedWidth || coordinate.y < 0 || coordinate.y >= ResolvedHeight)
        {
            return null;
        }
        return nodes[coordinate.x, coordinate.y];
    }
    #endregion
}
