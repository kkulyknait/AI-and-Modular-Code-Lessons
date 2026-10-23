using UnityEngine;

[RequireComponent(typeof(IntermediateAgentContext))]
public class IntermediatePerceptionSensor : MonoBehaviour
{
    #region Inspector Variables

    [SerializeField]
    private float perceptionRadius = 6f;

    [SerializeField]
    private float fieldOfView = 120f;

    [SerializeField]
    private float eyeHeight = 0.5f;

    [SerializeField]
    private LayerMask perceptionMask;

    [SerializeField]
    private LayerMask obstructionMask;

    #endregion

    #region Private Variables

    private IntermediateAgentContext context;

    #endregion

    #region Unity Methods

    /// <summary>
    /// Gets the IntermediateAgentContext required by the sensor.
    /// </summary>
    private void Awake()
    {
        context = GetComponent<IntermediateAgentContext>();
    }

    /// <summary>
    /// Refreshes the objects currently visible to the agent.
    /// </summary>
    private void Update()
    {
        context.ClearPerception();

        Collider[] detectedColliders = Physics.OverlapSphere(
            transform.position,
            perceptionRadius,
            perceptionMask,
            QueryTriggerInteraction.Collide);

        foreach (Collider detectedCollider in detectedColliders)
        {
            Transform perceivedTransform = GetPerceivedTransform(detectedCollider);

            if (perceivedTransform == null)
            {
                continue;
            }

            if (!IsInsideFieldOfView(perceivedTransform))
            {
                continue;
            }

            if (!HasLineOfSight(perceivedTransform))
            {
                continue;
            }

            StorePerceivedObject(perceivedTransform);
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Finds the tagged Transform represented by a detected collider.
    /// </summary>
    /// <param name="detectedCollider">The collider returned by OverlapSphere.</param>
    /// <returns>The tagged Transform represented by the collider.</returns>
    private Transform GetPerceivedTransform(Collider detectedCollider)
    {
        Transform currentTransform = detectedCollider.transform;

        while (currentTransform != null)
        {
            if (currentTransform.CompareTag("Key")
                || currentTransform.CompareTag("Coin")
                || currentTransform.CompareTag("Chest"))
            {
                return currentTransform;
            }

            currentTransform = currentTransform.parent;
        }

        return null;
    }

    /// <summary>
    /// Determines whether a Transform is inside the agent's field of view.
    /// </summary>
    /// <param name="target">The Transform being evaluated.</param>
    /// <returns>True when the target is inside the field of view.</returns>
    private bool IsInsideFieldOfView(Transform target)
    {
        Vector3 directionToTarget = target.position - transform.position;
        directionToTarget.y = 0f;

        if (directionToTarget.sqrMagnitude <= 0f)
        {
            return true;
        }

        float minimumDotProduct = Mathf.Cos(fieldOfView * 0.5f * Mathf.Deg2Rad);
        float targetDotProduct = Vector3.Dot(transform.forward, directionToTarget.normalized);

        return targetDotProduct >= minimumDotProduct;
    }

    /// <summary>
    /// Determines whether an obstacle blocks the target from view.
    /// </summary>
    /// <param name="target">The Transform being evaluated.</param>
    /// <returns>True when no obstacle blocks the target.</returns>
    private bool HasLineOfSight(Transform target)
    {
        Vector3 rayOrigin = transform.position + Vector3.up * eyeHeight;
        Vector3 targetPosition = target.position + Vector3.up * eyeHeight;
        Vector3 directionToTarget = targetPosition - rayOrigin;
        float distanceToTarget = directionToTarget.magnitude;

        return !Physics.Raycast(
            rayOrigin,
            directionToTarget.normalized,
            distanceToTarget,
            obstructionMask,
            QueryTriggerInteraction.Ignore);
    }

    /// <summary>
    /// Stores a visible object in the matching context field.
    /// </summary>
    /// <param name="perceivedTransform">The visible Transform.</param>
    private void StorePerceivedObject(Transform perceivedTransform)
    {
        if (perceivedTransform.CompareTag("Key"))
        {
            context.VisibleKey = perceivedTransform;
            return;
        }

        if (perceivedTransform.CompareTag("Coin"))
        {
            context.VisibleCoin = perceivedTransform;
            return;
        }

        if (perceivedTransform.CompareTag("Chest"))
        {
            context.VisibleChest = perceivedTransform;
        }
    }

    #endregion
}
