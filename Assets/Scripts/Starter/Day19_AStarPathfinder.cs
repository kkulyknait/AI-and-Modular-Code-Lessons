using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Searches a supplied grid with the A* algorithm and reconstructs a start-to-goal path.
/// </summary>
public class Day19_AStarPathfinder
{
    #region Private Variables
    private Day19_GridMap gridMap;
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a pathfinder for one searchable grid.
    /// </summary>
    public Day19_AStarPathfinder(Day19_GridMap gridMap)
    {
        this.gridMap = gridMap;
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Searches from the start node to the goal node.
    /// </summary>
    /// <returns>A start-to-goal path, or an empty list when no route exists.</returns>
    public List<Day19_GridNode> FindPath(Day19_GridNode startNode, Day19_GridNode goalNode)
    {
        List<Day19_GridNode> openNodes = new List<Day19_GridNode>();
        List<Day19_GridNode> closedNodes = new List<Day19_GridNode>();

        // TODO 2: Initialize and add the start node to the Open List.

        // TODO 3: Repeat while the Open List contains nodes.
        while (false)
        {
            // TODO 4: Select the lowest-cost Open node.
            Day19_GridNode currentNode = null;

            // TODO 5: Finish when the goal is selected.

            // TODO 6: Move the current node from Open to Closed.

            // TODO 7: Evaluate each neighbour.
        }

        return new List<Day19_GridNode>();
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Selects the node with the lowest F cost, using H as the tie-breaker.
    /// </summary>
    private Day19_GridNode GetLowestCostNode(List<Day19_GridNode> openNodes)
    {
        // TODO 8: Compare Open nodes and return the best candidate.
        return openNodes.Count > 0 ? openNodes[0] : null;
    }

    /// <summary>
    /// Calculates four-direction grid distance between two nodes.
    /// </summary>
    private int CalculateDistance(Day19_GridNode firstNode, Day19_GridNode secondNode)
    {
        // TODO 9: Calculate Manhattan distance.
        return 0;
    }

    /// <summary>
    /// Follows parent links from goal to start, then reverses the result.
    /// </summary>
    private List<Day19_GridNode> ReconstructPath(Day19_GridNode goalNode)
    {
        // TODO 10: Reconstruct the path from goal to start.
        return new List<Day19_GridNode>();
    }
    #endregion
}