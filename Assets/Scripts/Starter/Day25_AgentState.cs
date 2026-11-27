public abstract class Day25_AgentState
{
    #region Protected Variables

    protected Day25_StateController agent;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a state that operates on the supplied agent controller.
    /// </summary>
    /// <param name="agent">The controller that owns the state machine.</param>
    protected Day25_AgentState(Day25_StateController agent)
    {
        // Store the shared agent controller.
        this.agent = agent;
    }

    #endregion

    #region Public Properties

    public abstract string Name
    {
        get;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Performs one-time setup when this state becomes active.
    /// </summary>
    public abstract void Enter();

    /// <summary>
    /// Performs ongoing behaviour and evaluates outgoing transitions.
    /// </summary>
    public abstract void Update();

    /// <summary>
    /// Performs cleanup immediately before this state stops being active.
    /// </summary>
    public abstract void Exit();

    #endregion
}
