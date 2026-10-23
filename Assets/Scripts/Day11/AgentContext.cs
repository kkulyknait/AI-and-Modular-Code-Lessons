using UnityEngine;
using System.Collections.Generic;

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

    public List<Transform> PerceivedObjects = new List<Transform>();

    public Transform VisibleAgent;
    public Transform VisiblePickup;
    public Transform VisibleTarget;
    public Transform VisibleHazard;
    public bool HasKey;
    public bool HasResponseTarget;
    public Transform ResponseTarget;
    public Transform LastPerceivedObject;
    public Vector3 LastPerceivedPosition;
    public float LastPerceivedTime;

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
    //  Clear the current perception results
    public void ClearPerception()
    {
        PerceivedObjects.Clear();
        VisibleAgent = null;
        VisiblePickup = null;
        VisibleTarget = null;
        VisibleHazard = null;
    }

    public void AddPerceivedObject(Transform perceived)
    {
        if (PerceivedObjects.Contains(perceived))
        {
            return;
        }

        PerceivedObjects.Add(perceived);

        LastPerceivedObject = perceived;
        LastPerceivedPosition = perceived.position;
        LastPerceivedTime = Time.time;

        if (perceived.CompareTag("Player") || perceived.CompareTag("AIAgent"))
        {
            VisibleAgent = perceived;
        }
        else if (perceived.CompareTag("Key") || perceived.CompareTag("Powerup"))
        {
            VisiblePickup = perceived;
        }
        else if (perceived.CompareTag("Chest"))
        {
            VisibleTarget = perceived;
        }
        else if (perceived.CompareTag("Hazard"))
        {
            VisibleHazard = perceived;
        }
    }


    #endregion
}
