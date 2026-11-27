using UnityEngine;

public class Day25_StateController : MonoBehaviour
{
    #region Inspector Variables

    [SerializeField]
    private bool isPlayerVisible;

    [SerializeField]
    private bool isSearchComplete;

    #endregion

    #region Public Properties

    public bool IsPlayerVisible
    {
        get
        {
            return isPlayerVisible;
        }
    }

    public bool IsSearchComplete
    {
        get
        {
            return isSearchComplete;
        }
    }

    public Day25_AgentState PatrolState
    {
        get;
        private set;
    }

    public Day25_AgentState ChaseState
    {
        get;
        private set;
    }

    public Day25_AgentState SearchState
    {
        get;
        private set;
    }

    public Day25_AgentState CurrentState
    {
        get;
        private set;
    }

    #endregion

    #region Unity Methods

    /// <summary>
    /// Creates the available states before the state machine starts.
    /// </summary>
    private void Awake()
    {
        // Create one instance of each state.
        PatrolState = new Day25_PatrolState(this);
        ChaseState = new Day25_ChaseState(this);
        SearchState = new Day25_SearchState(this);
    }

    /// <summary>
    /// Enters the initial state.
    /// </summary>
    private void Start()
    {
        // Set Patrol as the initial state.
        TransitionTo(PatrolState, "Initial state");
    }

    /// <summary>
    /// Delegates behaviour to the one active state.
    /// </summary>
    private void Update()
    {
        // Update only the current state.
        if (CurrentState == null)
        {
            return;
        }
        CurrentState.Update();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Changes the current state using the consistent Exit, change, Enter lifecycle.
    /// </summary>
    /// <param name="nextState">The state that should become active.</param>
    /// <param name="reason">The condition that caused the transition.</param>
    public void TransitionTo(Day25_AgentState nextState, string reason)
    {
        // Reject null states and accidental re-entry.
        if(nextState == null || nextState == CurrentState)
        {
            return;
        }

        // Store the previous state name for the transition log.
        string previousStateName = CurrentState == null ? "None" : CurrentState.Name;

        // Exit the old state when one exists.
        if(CurrentState != null)
        {
            CurrentState.Exit();
        }

        // Change the current-state reference.
        CurrentState = nextState;

        // Log the transition.
        Day25_StateDebug.LogTransition(previousStateName, CurrentState.Name, reason);

        // Enter the new state.
        CurrentState.Enter();
    }

    #endregion
}
