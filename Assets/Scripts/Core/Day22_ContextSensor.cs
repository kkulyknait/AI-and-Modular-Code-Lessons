using UnityEngine;

[RequireComponent(typeof(Day22_AgentContext))]
public class Day22_ContextSensor : MonoBehaviour
{
    #region Inspector Variables

    [SerializeField]
    private float detectionDistance = 0.4f;

    [SerializeField]
    private float sensorRadius = 0.2f;

    [SerializeField]
    private float sensorHeight = 0.5f;

    [SerializeField]
    private LayerMask blockingMask;

    #endregion

    #region Private Variables

    private Day22_AgentContext context;

    #endregion

    #region Unity Methods

    /// <summary>
    /// Gets the shared Day 22 agent context.
    /// </summary>
    private void Awake()
    {
        context = GetComponent<Day22_AgentContext>();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Refreshes locally available directions before the agent makes a movement decision.
    /// </summary>
    public void RefreshAvailableDirections()
    {
        context.ClearAvailableDirections();
        AddDirectionIfAvailable(transform.forward);
        AddDirectionIfAvailable(transform.right);
        AddDirectionIfAvailable(-transform.forward);
        AddDirectionIfAvailable(-transform.right);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Adds a local direction when the SphereCast reports it as clear.
    /// </summary>
    private void AddDirectionIfAvailable(Vector3 direction)
    {
        if (!IsDirectionAvailable(direction))
        {
            return;
        }

        context.AddAvailableDirection(direction);
    }

    /// <summary>
    /// Checks a local direction for permanent or contextual obstacles.
    /// </summary>
    private bool IsDirectionAvailable(Vector3 direction)
    {
        Vector3 origin = transform.position + Vector3.up * sensorHeight;
        return !Physics.SphereCast(origin, sensorRadius, direction.normalized, out RaycastHit hit, detectionDistance, blockingMask, QueryTriggerInteraction.Collide);
    }

    #endregion
}
