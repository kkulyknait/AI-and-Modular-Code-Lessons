public class Day25_PatrolState : Day25_AgentState
{
    #region Constructors

    /// <summary>
    /// Creates the Patrol state.
    /// </summary>
    public Day25_PatrolState(Day25_StateController agent) : base(agent)
    {
    }

    #endregion

    #region Public Properties

    public override string Name
    {
        get
        {
            return "Patrol";
        }
    }

    #endregion

    #region Public Methods

    public override void Enter()
    {
        // Log the active Patrol behaviour.
        Day25_StateDebug.LogEnter(Name, "Following the patrol path and watching for the player.");
    }

    public override void Update()
    {
        // Transition to Chase when the player becomes visible.
        if(agent.IsPlayerVisible)
        {
            agent.TransitionTo(agent.ChaseState, "Player became visible!  I see you!");
            return;
        }
    }

    public override void Exit()
    {
        // Log that Patrol ended.
        Day25_StateDebug.LogExit(Name);
    }

    #endregion
}
