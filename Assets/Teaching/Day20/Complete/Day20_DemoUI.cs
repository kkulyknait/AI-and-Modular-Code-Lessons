using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Displays Day 20 resolution/search statistics and connects the Play Mode button.
/// Uses a TextMeshPro text object for the statistics (import TMP Essentials if Unity asks).
/// </summary>
public class Day20_DemoUI : MonoBehaviour
{
    #region Inspector Variables
    [SerializeField] private Day20_DemoController controller;
    [SerializeField] private Button increaseResolutionButton;
    [SerializeField] private TMP_Text statisticsText;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        if (controller == null)
        {
            controller = FindFirstObjectByType<Day20_DemoController>();
        }

        if (increaseResolutionButton != null)
        {
            increaseResolutionButton.onClick.AddListener(OnIncreaseResolutionClicked);
        }
    }
    #endregion

    #region Public Methods
    public void Refresh(Day20_DemoController activeController)
    {
        controller = activeController;
        Day20_GridMap grid = controller.GridMap;

        if (statisticsText != null)
        {
            statisticsText.text =
                $"Resolution: {grid.Resolution}\n" +
                $"Grid: {grid.ResolvedWidth} x {grid.ResolvedHeight}\n" +
                $"Total Nodes: {grid.TotalNodeCount}\n" +
                $"Walkable Nodes: {grid.WalkableNodeCount}\n" +
                $"Path Nodes: {controller.PathNodeCount}\n" +
                $"Opened Nodes: {controller.OpenedNodeCount}\n" +
                $"Closed Nodes: {controller.ClosedNodeCount}\n" +
                $"Search Time: {controller.SearchMilliseconds:F4} ms";
        }

        if (increaseResolutionButton != null)
        {
            increaseResolutionButton.interactable = grid.CanIncreaseResolution;
        }
    }
    #endregion

    #region Private Methods
    private void OnIncreaseResolutionClicked()
    {
        controller?.IncreaseResolutionAndRun();
    }
    #endregion
}
