using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Runs the supplied grid through the student A* implementation once at startup.
/// </summary>
[RequireComponent(typeof(Day19_GridMap))]
public class Day19_DemoController : MonoBehaviour
{
    #region Private Variables
    private Day19_GridMap gridMap;
    #endregion

    #region Unity Methods
    /// <summary>
    /// Finds and displays a path after all scene objects are initialized.
    /// </summary>
    private void Start()
    {
        gridMap = GetComponent<Day19_GridMap>();
        RunSearch();
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Resets the board, runs A*, and sends the result back to the grid display.
    /// </summary>
    public void RunSearch()
    {
        gridMap.ResetSearch();
        Day19_AStarPathfinder pathfinder = new Day19_AStarPathfinder(gridMap);
        List<Day19_GridNode> path = pathfinder.FindPath(gridMap.StartNode, gridMap.GoalNode);
        gridMap.SetPath(path);
        Debug.Log(path.Count > 0 ? $"Path found with {path.Count} nodes." : "No path found.");
    }
    #endregion
}