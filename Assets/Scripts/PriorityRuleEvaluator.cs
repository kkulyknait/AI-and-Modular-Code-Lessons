using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor.Rendering;


public class PriorityRuleEvaluator : MonoBehaviour
{
    [Header("Game State References")]
    [SerializeField] private Transform playerTransform;

    [Header("AI State Variables")]
    [SerializeField] private float currentHealth = 100f;
    [SerializeField] private float maxHealth = 100f;

    [Header("Rule Thresholds")]
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float chaseRange = 15f;
    [SerializeField] private float lowHealthThreshold = 0.2f;  // 20% of max health

    private enum AIAction { Patrol, Chase, Attack, Flee }
    //  Pair each of these with it's priority using small struct
    private struct CandidateAction
    {
        public AIAction Action;
        public int Priority;//  Higher number, higher priority

        public CandidateAction(AIAction action, int priority)
        {
            Action = action;
            Priority = priority;

        }

    }
    [SerializeField] private AIAction currentAction = AIAction.Patrol;

    private void Update()
    {
        EvaluateRuleset();
    }

    private void EvaluateRuleset()
    {
        //  Gather our Data
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        float healthPercentage = currentHealth / maxHealth;

        //  Collect every Action that's true right now
        List<CandidateAction> candidates = new List<CandidateAction>();

        //  Rule 1  - Flee when health is low
        if (healthPercentage <= lowHealthThreshold)
        {
            candidates.Add(new CandidateAction(AIAction.Flee, 4));
        }

        //  Rule 2 - Attack when player is in range
        if (distanceToPlayer <= attackRange)
        {
            candidates.Add(new CandidateAction(AIAction.Attack, 3));
        }

        //  Rule 3 - Chase when player is in range but not close enough to attack
        if (distanceToPlayer <= chaseRange)
        {
            candidates.Add(new CandidateAction(AIAction.Chase, 2));
        }

        candidates.Add(new CandidateAction(AIAction.Patrol, 1)); //  Default action

        //  Sort candidates by priority, then pick one
        CandidateAction chosen = candidates[0];
        foreach (CandidateAction candidate in candidates)
        {
            if (candidate.Priority > chosen.Priority)
            {
                chosen = candidate;
            }
        }
        SetAction(chosen.Action);
    }

    private void SetAction(AIAction newAction)
    {
     //  only do this if action actually changes
     if (currentAction != newAction)
        {
            currentAction = newAction;
            Debug.Log($"Ruleset Evaluated. New action: {currentAction}");
        }
     
    }

}
