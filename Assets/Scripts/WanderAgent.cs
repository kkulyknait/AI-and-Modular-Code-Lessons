using UnityEngine;
[RequireComponent(typeof(Rigidbody))]

public class WanderAgent : MonoBehaviour
{
    [SerializeField] private float wanderRadius = 8f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotationSpeed = 18f;

    [Header("Perception Settings")]
    [SerializeField] private float fieldOfView = 0.5f;  //  0.5f roughly equal to 90 degree cone

    private Rigidbody _rb;
    private Vector3 _targetPosition;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        SetNewRandomTarget();
    }

    void Start()
    {

    }


    private void FixedUpdate()
    {
        MoveAndRotate();
        CheckDestinationReached();
    }

    private void SetNewRandomTarget()
    {
        // Find random point inside wander radius
        Vector2 randomPoint = Random.insideUnitCircle * wanderRadius;

        //  convert 2d point into a 3d position
        _targetPosition = new Vector3(randomPoint.x, transform.position.y, randomPoint.y);
    }

    private void MoveAndRotate()
    {

        //  vector = target - origion
        Vector3 directionToTarget = _targetPosition - transform.position;

        if (directionToTarget.sqrMagnitude > 0.1f) // Check if the target is far enough
        {
            // normalize direction so we have consistent speed
            directionToTarget.Normalize();
            //  Quaternions...calculate the rotation needed to look at the target
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            //  Smoothly rotate towards the target rotation
            Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, targetRotation,
                rotationSpeed * Time.fixedDeltaTime);
            _rb.MoveRotation(newRotation);


        }
        // Physics Movement: Move forward along the Z axis based on current rotation
        Vector3 movement = transform.forward * moveSpeed * Time.fixedDeltaTime;
        _rb.MovePosition(transform.position + movement);
    }

    private void CheckDestinationReached()
    {
        // Vectors: Check the distance between current position and target
        if (Vector3.Distance(transform.position, _targetPosition) < 0.5f)
        {
            SetNewRandomTarget();
        }
    }

    // Triggers and Dot Product: Simulating AI Vision
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Vectors: Get the direction specifically to the player
            Vector3 directionToPlayer = (other.transform.position - transform.position).normalized;

            // Vectors: Calculate Dot Product between where we are looking and where the player is
            float dotProduct = Vector3.Dot(transform.forward, directionToPlayer);

            // If the dot product is greater than our FOV threshold, the player is in front of us
            if (dotProduct > fieldOfView)
            {
                Debug.Log("Player is within line of sight!");

                // Draw a debug line in the editor to visualize the line of sight
                Debug.DrawLine(transform.position, other.transform.position, Color.red);
            }


        }
    }
}
