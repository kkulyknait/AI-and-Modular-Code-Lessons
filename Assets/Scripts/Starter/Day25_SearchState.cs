public class Day25_SearchState : Day25_AgentState
{
    #region Constructors

    /// <summary>
    /// Creates the Search state.
    /// </summary>
    public Day25_SearchState(Day25_StateController agent) : base(agent)
    {
    }

    #endregion

    #region Public Properties

    public override string Name
    {
        get
        {
            return "Search";
        }
    }

    #endregion

    #region Public Methods

    public override void Enter()
    {
        // Log the active Search behaviour.
        Day25_StateDebug.LogEnter(Name, "Investigating Players last known location.");
    }

    public override void Update()
    {
        // Give Player Visible priority over Search Complete.
        if(agent.IsPlayerVisible)
        {
            agent.TransitionTo(agent.ChaseState, "Found you!  Initiating Chase!");
            return;
        }
        if(agent.IsSearchComplete)
        {
            agent.TransitionTo(agent.PatrolState, " Search completed without finding the player");
            return;
        }
    }

    public override void Exit()
    {
        // Log that Search ended.
        Day25_StateDebug.LogExit(Name);
    }

    #endregion
}
