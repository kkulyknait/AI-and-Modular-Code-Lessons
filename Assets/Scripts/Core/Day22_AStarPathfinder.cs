using System.Collections.Generic;
using UnityEngine;

public class Day22_AStarPathfinder : MonoBehaviour
{
    #region Public Methods

    /// <summary>
    /// Finds a path between two walkable nodes using A*.
    /// </summary>
    /// <param name="pathGrid">The grid searched by A*.</param>
    /// <param name="startNode">The node representing the Human position.</param>
    /// <param name="goalNode">The node representing the Chest position.</param>
    /// <returns>The reconstructed path, or an empty list when no path exists.</returns>
    public List<Day22_GridNode> FindPath(Day22_PathGrid pathGrid, Day22_GridNode startNode, Day22_GridNode goalNode)
    {
        List<Day22_GridNode> openNodes = new List<Day22_GridNode>();

        if (pathGrid == null || startNode == null || goalNode == null || !startNode.IsWalkable || !goalNode.IsWalkable)
        {
            return new List<Day22_GridNode>();
        }

        pathGrid.ResetSearch();
        startNode.GCost = 0;
        startNode.HCost = CalculateDistance(startNode, goalNode);
        startNode.CalculateFCost();
        startNode.IsOpen = true;
        openNodes.Add(startNode);

        while (openNodes.Count > 0)
        {
            Day22_GridNode currentNode = GetLowestCostNode(openNodes);

            if (currentNode == goalNode)
            {
                return ReconstructPath(goalNode);
            }

            openNodes.Remove(currentNode);
            currentNode.IsOpen = false;
            currentNode.IsClosed = true;

            foreach (Day22_GridNode neighbourNode in pathGrid.GetNeighbours(currentNode))
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

        return new List<Day22_GridNode>();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Selects the Open node with the lowest F cost and lowest H cost tie breaker.
    /// </summary>
    private Day22_GridNode GetLowestCostNode(List<Day22_GridNode> openNodes)
    {
        Day22_GridNode bestNode = openNodes[0];

        foreach (Day22_GridNode candidateNode in openNodes)
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
    private int CalculateDistance(Day22_GridNode firstNode, Day22_GridNode secondNode)
    {
        int horizontalDistance = Mathf.Abs(firstNode.Coordinate.x - secondNode.Coordinate.x);
        int verticalDistance = Mathf.Abs(firstNode.Coordinate.y - secondNode.Coordinate.y);
        return horizontalDistance + verticalDistance;
    }

    /// <summary>
    /// Reconstructs the Start-to-Goal path through Parent links.
    /// </summary>
    private List<Day22_GridNode> ReconstructPath(Day22_GridNode goalNode)
    {
        List<Day22_GridNode> path = new List<Day22_GridNode>();
        Day22_GridNode currentNode = goalNode;

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
