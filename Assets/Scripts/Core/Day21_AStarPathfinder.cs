using System.Collections.Generic;
using UnityEngine;

public class Day21_AStarPathfinder
{
    #region Private Variables

    private Day21_PathGrid pathGrid;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates an A* pathfinder that searches a Day 21 path grid.
    /// </summary>
    /// <param name="pathGrid">The grid searched by A*.</param>
    public Day21_AStarPathfinder(Day21_PathGrid pathGrid)
    {
        this.pathGrid = pathGrid;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Finds a path between two walkable nodes using A*.
    /// </summary>
    /// <param name="startNode">The node representing the agent position.</param>
    /// <param name="goalNode">The node representing the goal position.</param>
    /// <returns>The reconstructed path, or an empty list when no path exists.</returns>
    public List<Day21_GridNode> FindPath(Day21_GridNode startNode, Day21_GridNode goalNode)
    {
        List<Day21_GridNode> openNodes = new List<Day21_GridNode>();

        if (startNode == null || goalNode == null || !startNode.IsWalkable || !goalNode.IsWalkable)
        {
            return new List<Day21_GridNode>();
        }

        pathGrid.ResetSearch();
        startNode.GCost = 0;
        startNode.HCost = CalculateDistance(startNode, goalNode);
        startNode.CalculateFCost();
        startNode.IsOpen = true;
        openNodes.Add(startNode);

        while (openNodes.Count > 0)
        {
            Day21_GridNode currentNode = GetLowestCostNode(openNodes);

            if (currentNode == goalNode)
            {
                return ReconstructPath(goalNode);
            }

            openNodes.Remove(currentNode);
            currentNode.IsOpen = false;
            currentNode.IsClosed = true;

            foreach (Day21_GridNode neighbourNode in pathGrid.GetNeighbours(currentNode))
            {
                if (neighbourNode.IsClosed)
                {
                    continue;
                }

                int candidateGCost = currentNode.GCost + CalculateDistance(currentNode, neighbourNode);

                if (candidateGCost < neighbourNode.GCost)
                {
                    neighbourNode.Parent = currentNode;
                    neighbourNode.GCost = candidateGCost;
                    neighbourNode.HCost = CalculateDistance(neighbourNode, goalNode);
                    neighbourNode.CalculateFCost();
                }

                if (!neighbourNode.IsOpen)
                {
                    neighbourNode.IsOpen = true;
                    openNodes.Add(neighbourNode);
                }
            }
        }

        return new List<Day21_GridNode>();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Selects the Open node with the lowest F cost and lowest H cost tie breaker.
    /// </summary>
    /// <param name="openNodes">The discovered nodes not yet explored.</param>
    /// <returns>The next node selected for exploration.</returns>
    private Day21_GridNode GetLowestCostNode(List<Day21_GridNode> openNodes)
    {
        Day21_GridNode bestNode = openNodes[0];

        foreach (Day21_GridNode candidateNode in openNodes)
        {
            if (candidateNode.FCost < bestNode.FCost || candidateNode.FCost == bestNode.FCost && candidateNode.HCost < bestNode.HCost)
            {
                bestNode = candidateNode;
            }
        }

        return bestNode;
    }

    /// <summary>
    /// Calculates Manhattan grid distance between two nodes.
    /// </summary>
    /// <param name="firstNode">The first node.</param>
    /// <param name="secondNode">The second node.</param>
    /// <returns>The grid distance between the nodes.</returns>
    private int CalculateDistance(Day21_GridNode firstNode, Day21_GridNode secondNode)
    {
        int horizontalDistance = Mathf.Abs(firstNode.Coordinate.x - secondNode.Coordinate.x);
        int verticalDistance = Mathf.Abs(firstNode.Coordinate.y - secondNode.Coordinate.y);
        return horizontalDistance + verticalDistance;
    }

    /// <summary>
    /// Reconstructs the Start-to-Goal path through Parent links.
    /// </summary>
    /// <param name="goalNode">The reached Goal node.</param>
    /// <returns>The final path in Start-to-Goal order.</returns>
    private List<Day21_GridNode> ReconstructPath(Day21_GridNode goalNode)
    {
        List<Day21_GridNode> path = new List<Day21_GridNode>();
        Day21_GridNode currentNode = goalNode;

        while (currentNode != null)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }

        path.Reverse();
        return path;
    }

    #endregion
}
