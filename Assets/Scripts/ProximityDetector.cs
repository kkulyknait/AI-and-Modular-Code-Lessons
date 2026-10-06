using UnityEngine;

public class ProximityDetector : MonoBehaviour
{
    [Header("Dectection Settings")]
    [SerializeField] private string detectableTag = "Hazard";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(detectableTag))

        {
            Debug.Log($"{gameObject.name} detected a {detectableTag}:{other.gameObject.name}");
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(detectableTag))
        {
            Debug.Log($"{gameObject.name} lost detection of a {detectableTag}:{other.gameObject.name}");
        }
    }
}
