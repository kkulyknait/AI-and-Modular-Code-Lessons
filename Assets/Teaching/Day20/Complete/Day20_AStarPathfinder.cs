using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Runs the completed Day 19 A* process against the resolved Day 20 grid.
/// </summary>
public class Day20_AStarPathfinder
{
    #region Private Variables
    private Day20_GridMap gridMap;
    #endregion

    #region Public Properties
    public int NodesAddedToOpen { get; private set; }
    public int NodesClosed { get; private set; }
    #endregion

    #region Constructors
    public Day20_AStarPathfinder(Day20_GridMap gridMap)
    {
        this.gridMap = gridMap;
    }
    #endregion

    #region Public Methods
    public List<Day20_GridNode> FindPath(Day20_GridNode startNode, Day20_GridNode goalNode)
    {
        List<Day20_GridNode> openNodes = new List<Day20_GridNode>();
        List<Day20_GridNode> closedNodes = new List<Day20_GridNode>();
        NodesAddedToOpen = 0;
        NodesClosed = 0;

        startNode.GCost = 0;
        startNode.HCost = CalculateDistance(startNode, goalNode);
        startNode.CalculateFCost();
        startNode.IsOpen = true;
        openNodes.Add(startNode);
        NodesAddedToOpen++;

        while (openNodes.Count > 0)
        {
            Day20_GridNode currentNode = GetLowestCostNode(openNodes);

            if (currentNode == goalNode)
            {
                return ReconstructPath(goalNode);
            }

            openNodes.Remove(currentNode);
            currentNode.IsOpen = false;
            currentNode.IsClosed = true;
            closedNodes.Add(currentNode);
            NodesClosed++;

            List<Day20_GridNode> neighbours = gridMap.GetNeighbours(currentNode);
            foreach (Day20_GridNode neighbourNode in neighbours)
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
                    NodesAddedToOpen++;
                }
            }
        }

        return new List<Day20_GridNode>();
    }
    #endregion

    #region Private Methods
    private Day20_GridNode GetLowestCostNode(List<Day20_GridNode> openNodes)
    {
        Day20_GridNode bestNode = openNodes[0];
        foreach (Day20_GridNode candidateNode in openNodes)
        {
            if (candidateNode.FCost < bestNode.FCost || candidateNode.FCost == bestNode.FCost && candidateNode.HCost < bestNode.HCost)
            {
                bestNode = candidateNode;
            }
        }
        return bestNode;
    }

    private int CalculateDistance(Day20_GridNode firstNode, Day20_GridNode secondNode)
    {
        return Mathf.Abs(firstNode.Coordinate.x - secondNode.Coordinate.x) +
               Mathf.Abs(firstNode.Coordinate.y - secondNode.Coordinate.y);
    }

    private List<Day20_GridNode> ReconstructPath(Day20_GridNode goalNode)
    {
        List<Day20_GridNode> path = new List<Day20_GridNode>();
        Day20_GridNode currentNode = goalNode;
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
