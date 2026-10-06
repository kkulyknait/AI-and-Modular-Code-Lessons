using UnityEngine;

[RequireComponent(typeof(AgentContext))]
public class DetectionAgent : MonoBehaviour
{
    #region Private Variables

    private AgentContext context;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        context = GetComponent<AgentContext>();
    }

    /// <summary>
    /// Called when the character controller hits a collider while performing a Move.
    /// </summary>
    /// <param name="hit"></param>
    private void OnControllerColliderHit(
        ControllerColliderHit hit)
    {
        DetectionSource source = hit.collider.GetComponent<DetectionSource>();

        if (source == null || source.Type != DetectionType.Obstacle)
        {
            return;
        }

        context.IsNearObstacle = true;
        context.CurrentDetection = DetectionType.Obstacle;

        Debug.Log($"Player contacted obstacle: {hit.gameObject.name}", this);

        context.PrintCurrentState();
    }

    private void OnCollisionEnter(
        Collision collision)
    {
        DetectionSource source = collision.collider.GetComponent<DetectionSource>();

        if (source == null || source.Type != DetectionType.Obstacle)
        {
            return;
        }

        context.IsNearObstacle = true;
        context.CurrentDetection = DetectionType.Obstacle;

        Debug.Log($"Agent contacted obstacle: {collision.gameObject.name}", this);

        context.PrintCurrentState();
    }

    private void OnCollisionExit(Collision collision)
    {
        DetectionSource source = collision.collider.GetComponent<DetectionSource>();

        if (source == null || source.Type != DetectionType.Obstacle)
        {
            return;
        }

        context.IsNearObstacle = false;
        RefreshCurrentDetection();

        Debug.Log($"Agent cleared obstacle: {collision.gameObject.name}", this);

        context.PrintCurrentState();
    }

    private void OnTriggerEnter(Collider other)
    {
        DetectionSource source = other.GetComponent<DetectionSource>();

        if (source == null)
        {
            return;
        }

        if (source.Type == DetectionType.Hazard)
        {
            context.IsNearHazard = true;
            context.CurrentDetection = DetectionType.Hazard;

            Debug.Log($"Hazard detected: {other.gameObject.name}", this);
        }
        else if (source.Type == DetectionType.Target)
        {
            context.IsNearTarget = true;
            context.CurrentTarget = other.gameObject;
            context.CurrentDetection = DetectionType.Target;

            Debug.Log($"Target detected: {other.gameObject.name}", this);
        }

        context.PrintCurrentState();
    }

    private void OnTriggerExit(
        Collider other)
    {
        DetectionSource source = other.GetComponent<DetectionSource>();

        if (source == null)
        {
            return;
        }

        if (source.Type == DetectionType.Hazard)
        {
            context.IsNearHazard = false;

            Debug.Log($"Hazard cleared: {other.gameObject.name}", this);
        }
        else if (source.Type == DetectionType.Target)
        {
            context.IsNearTarget = false;

            Debug.Log($"Target proximity cleared: {other.gameObject.name}", this);
        }

        RefreshCurrentDetection();
        context.PrintCurrentState();
    }

    #endregion

    #region Private Methods

    private void RefreshCurrentDetection()
    {
        if (context.IsNearTarget)
        {
            context.CurrentDetection = DetectionType.Target;
        }
        else if (context.IsNearHazard)
        {
            context.CurrentDetection = DetectionType.Hazard;
        }
        else if (context.IsNearObstacle)
        {
            context.CurrentDetection = DetectionType.Obstacle;
        }
        else
        {
            context.CurrentDetection = DetectionType.None;
        }
    }

    #endregion
}
