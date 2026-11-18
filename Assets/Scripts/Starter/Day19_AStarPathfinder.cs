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
        startNode.GCost = 0;
        startNode.HCost = CalculateDistance(startNode, goalNode);
        startNode.CalculateFCost();
        startNode.IsOpen = true;
        openNodes.Add(startNode);

        // TODO 3: Repeat while the Open List contains nodes.
        while (openNodes.Count > 0)
        {
            // TODO 4: Select the lowest-cost Open node.
            Day19_GridNode currentNode = GetLowestCostNode(openNodes);

            // TODO 5: Finish when the goal is selected.
            if (currentNode == goalNode)
            {
                return ReconstructPath(goalNode);
            }

            // TODO 6: Move the current node from Open to Closed.
            openNodes.Remove(currentNode);
            currentNode.IsOpen = false;
            currentNode.IsClosed = true;
            closedNodes.Add(currentNode);

            // TODO 7: Evaluate each neighbour.
            List<Day19_GridNode> neighbours = gridMap.GetNeighbours(currentNode);
            foreach (Day19_GridNode neighbourNode in neighbours)
            {
                if (neighbourNode.IsClosed)
                {
                    continue;
                }
                int candidateGCost = currentNode.GCost +
                    CalculateDistance(currentNode, neighbourNode);
                if(candidateGCost < neighbourNode.GCost)
                {
                    neighbourNode.Parent = currentNode;
                    neighbourNode.GCost = candidateGCost;
                    neighbourNode.HCost = CalculateDistance(neighbourNode, goalNode);
                    neighbourNode.CalculateFCost();
                    if (!neighbourNode.IsOpen)
                    {
                        neighbourNode.IsOpen = true;
                        openNodes.Add(neighbourNode);
                    }
                }
            }

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
        Day19_GridNode bestNode = openNodes[0];
        foreach (Day19_GridNode candidateNode in openNodes)
        {
            if (candidateNode.FCost < bestNode.FCost ||
           candidateNode.FCost == bestNode.FCost &&
            candidateNode.HCost < bestNode.HCost)
            {
                bestNode = candidateNode;
            }
        }
        return bestNode;
    }

    /// <summary>
    /// Calculates four-direction grid distance between two nodes.
    /// </summary>
    private int CalculateDistance(Day19_GridNode firstNode, Day19_GridNode secondNode)
    {
        // TODO 9: Calculate Manhattan distance.
        int horizontalDistance = Mathf.Abs(firstNode.Coordinate.x - secondNode.Coordinate.x);
        int verticalDistance = Mathf.Abs(firstNode.Coordinate.y - secondNode.Coordinate.y);

        return horizontalDistance + verticalDistance;
    }

    /// <summary>
    /// Follows parent links from goal to start, then reverses the result.
    /// </summary>
    private List<Day19_GridNode> ReconstructPath(Day19_GridNode goalNode)
    {
        // TODO 10: Reconstruct the path from goal to start.
        List<Day19_GridNode> path = new List<Day19_GridNode>();
        Day19_GridNode currentNode = goalNode;
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