using UnityEngine;

public class AgentContext : MonoBehaviour
{
    #region Public Variables

    public bool IsNearObstacle;

    public bool IsNearHazard;

    public bool IsNearTarget;

    public bool IsAvoiding;

    public bool IsThreatDetected;

    public GameObject CurrentTarget;

    public DetectionType CurrentDetection;

    public Vector3 AvoidanceDirection;
    
    public Vector3 ThreatDirection;
    public float ThreatDistance;

    #endregion

    #region Public Methods

    /// <summary>
    /// Prints the current environmental and avoidance context.
    /// </summary>
    public void PrintCurrentState()
    {
        string targetName =
            CurrentTarget == null
                ? "None"
                : CurrentTarget.name;

        Debug.Log(
            $"Detection: {CurrentDetection} | " +
            $"Is Near Obstacle: {IsNearObstacle} | " +
            $"Is Near Hazard: {IsNearHazard} | " +
            $"Is Near Target: {IsNearTarget} | " +
            $"Is Avoiding: {IsAvoiding} | " + 
            $"Avoidance Direction: {AvoidanceDirection} | " +
            $"Current Target: {targetName}", this);
    }

    #endregion
}
