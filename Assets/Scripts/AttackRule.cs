using UnityEngine;

public class AttackRule : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float attackRange = 5f;
    

    // Update is called once per frame
    void Update()
    {
        EvaluateAttackRule();
    }

    private void EvaluateAttackRule()
    {
        //  Input gathering
        float playerDistance = Vector3.Distance(transform.position, playerTransform.position);
        bool playerDetected = playerTransform != null; 

        //  Condition.  Binary logic.
        if (playerDetected && playerDistance <= attackRange)
        {
            // Action.  Output
            AttackPlayer();
        }
    }

    private void AttackPlayer()
    {
        Debug.Log("Attack the player!!");
    }

}
