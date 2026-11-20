using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

/// <summary>
/// Runs A*, captures comparison statistics, and handles resolution increases.
/// </summary>
[RequireComponent(typeof(Day20_GridMap))]
public class Day20_DemoController : MonoBehaviour
{
    #region Private Variables
    private Day20_GridMap gridMap;
    private Day20_DemoUI demoUI;
    #endregion

    #region Public Properties
    public Day20_GridMap GridMap => gridMap;
    public int PathNodeCount { get; private set; }
    public int OpenedNodeCount { get; private set; }
    public int ClosedNodeCount { get; private set; }
    public double SearchMilliseconds { get; private set; }
    #endregion

    #region Unity Methods
    private void Start()
    {
        gridMap = GetComponent<Day20_GridMap>();
        demoUI = FindFirstObjectByType<Day20_DemoUI>();
        RunSearch();
    }
    #endregion

    #region Public Methods
    public void RunSearch()
    {
        gridMap.ResetSearch();
        Day20_AStarPathfinder pathfinder = new Day20_AStarPathfinder(gridMap);
        Stopwatch stopwatch = Stopwatch.StartNew();
        List<Day20_GridNode> path = pathfinder.FindPath(gridMap.StartNode, gridMap.GoalNode);
        stopwatch.Stop();

        gridMap.SetPath(path);
        PathNodeCount = path.Count;
        OpenedNodeCount = pathfinder.NodesAddedToOpen;
        ClosedNodeCount = pathfinder.NodesClosed;
        SearchMilliseconds = stopwatch.Elapsed.TotalMilliseconds;
        demoUI?.Refresh(this);

        Debug.Log(path.Count > 0
            ? $"Resolution {gridMap.Resolution}: path {PathNodeCount} nodes, closed {ClosedNodeCount}, search {SearchMilliseconds:F4} ms."
            : $"Resolution {gridMap.Resolution}: no path found, search {SearchMilliseconds:F4} ms.");
    }

    public void IncreaseResolutionAndRun()
    {
        if (gridMap.IncreaseResolution())
        {
            RunSearch();
        }
        else
        {
            demoUI?.Refresh(this);
        }
    }
    #endregion
}
