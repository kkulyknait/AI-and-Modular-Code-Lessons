using UnityEngine;

public static class Day25_StateDebug
{
    #region Public Methods

    /// <summary>
    /// Logs that an agent entered a state and identifies the state's active behaviour.
    /// </summary>
    /// <param name="stateName">The state being entered.</param>
    /// <param name="behaviourDescription">The behaviour associated with the state.</param>
    public static void LogEnter(string stateName, string behaviourDescription)
    {
        Debug.Log($"ENTER {stateName}: {behaviourDescription}");
    }

    /// <summary>
    /// Logs that an agent exited a state.
    /// </summary>
    /// <param name="stateName">The state being exited.</param>
    public static void LogExit(string stateName)
    {
        Debug.Log($"EXIT {stateName}");
    }

    /// <summary>
    /// Logs a state transition and the condition that caused it.
    /// </summary>
    /// <param name="previousStateName">The state being left.</param>
    /// <param name="nextStateName">The state being entered.</param>
    /// <param name="reason">The transition condition that succeeded.</param>
    public static void LogTransition(string previousStateName, string nextStateName, string reason)
    {
        Debug.Log($"TRANSITION {previousStateName} -> {nextStateName}: {reason}");
    }

    #endregion
}
