using UnityEngine;

[RequireComponent(typeof(AgentContext))]
public class ThreatDetector : MonoBehaviour
{
    #region Inspector Variables
    [SerializeField] private Transform threat;
    [SerializeField] float detectionRadius = 5f;
    #endregion

    #region Private Variables
    private AgentContext context;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        context = GetComponent<AgentContext>();
    }

    private void Update()
    {
        if (threat == null)
        {
            ClearThreat();
            return;
        }

        Vector3 toThreat = threat.position - transform.position;
        toThreat.y = 0f;  // Ignore vertical distance
        float distance = toThreat.magnitude;

        if (distance > detectionRadius || distance <= 0f)   
        {
            ClearThreat();
            return;
        }

        context.IsThreatDetected = true;
        context.ThreatDistance = distance;
        context.ThreatDirection = toThreat.normalized;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
    #endregion
    #region Private Methods
    private void ClearThreat()
    {
        context.IsThreatDetected = false;
        context.ThreatDistance = 0f;
        context.ThreatDirection = Vector3.zero;
    }
    #endregion

}
