public class Day25_ChaseState : Day25_AgentState
{
    #region Constructors

    /// <summary>
    /// Creates the Chase state.
    /// </summary>
    public Day25_ChaseState(Day25_StateController agent) : base(agent)
    {
    }

    #endregion

    #region Public Properties

    public override string Name
    {
        get
        {
            return "Chase";
        }
    }

    #endregion

    #region Public Methods

    public override void Enter()
    {
        // Log the active Chase behaviour.
        Day25_StateDebug.LogEnter(Name, "Chasing the player using the agent's movement capabilities.");
    }

    public override void Update()
    {
        // Transition to Search when the player is no longer visible.
        if(!agent.IsPlayerVisible)
        {
            agent.TransitionTo(agent.SearchState, "Player is no longer visible!  Where did they go?  I'll find you dammit!");
            return;
        }
    }

    public override void Exit()
    {
        // Log that Chase ended.
        Day25_StateDebug.LogExit(Name);
    }

    #endregion
}
