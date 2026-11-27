# Day 25 Walkthrough: Implementing a Finite State Machine

## Supplied Scene

The scene requires only one empty GameObject:

```text
State Machine Demo
└── Day25_StateController
```

No movement, perception, colliders, or player input are required. `Player Visible` and `Search Complete` are controlled directly in the Inspector during Play Mode. The Console provides the visible output.

# Part 1: Review the Design Documents

Open:

```text
Documentation/day25-state-transition-table.md
Documentation/day25-fsm-flowchart.png
```

Confirm the three required states:

```text
Patrol
Chase
Search
```

Trace the four transitions before opening the code.

# Part 2: Implement the Base State Constructor

Open:

```text
Day25/Starter/day25_agent-state.cs
```

Locate the constructor and find:

```csharp
// Store the shared agent controller.
```

Add directly below:

```csharp
this.agent = agent;
```

Review the supplied abstract lifecycle:

```csharp
public abstract void Enter();
public abstract void Update();
public abstract void Exit();
```

## Compile Checkpoint

Save and confirm the project compiles.

# Part 3: Create the Available States

Open:

```text
Day25/Starter/day25_state-controller.cs
```

Locate `Awake()` and find:

```csharp
// Create one instance of each state.
```

Add:

```csharp
PatrolState = new Day25_PatrolState(this);
ChaseState = new Day25_ChaseState(this);
SearchState = new Day25_SearchState(this);
```

## Discussion

Each state receives the same controller, which provides shared agent information and access to the other states.

# Part 4: Set the Initial State

Locate `Start()` and find:

```csharp
// Set Patrol as the initial state.
```

Add:

```csharp
TransitionTo(PatrolState, "Initial state");
```

`TransitionTo()` is not finished yet, but this establishes the intended starting state.

# Part 5: Delegate Update to Current State

Locate `Update()` and find:

```csharp
// Update only the current state.
```

Add:

```csharp
if (CurrentState == null)
{
    return;
}

CurrentState.Update();
```

## Teaching Point

Unity updates the controller. The controller delegates to exactly one active state. The controller does not contain Patrol, Chase, or Search behaviour.

# Part 6: Validate Transitions

Locate `TransitionTo()` and find:

```csharp
// Reject null states and accidental re-entry.
```

Add:

```csharp
if (nextState == null || nextState == CurrentState)
{
    return;
}
```

This prevents obvious invalid transitions and repeated entry every frame.

# Part 7: Store the Previous State Name

Find:

```csharp
// Store the previous state name for the transition log.
```

Add:

```csharp
string previousStateName = CurrentState == null ? "None" : CurrentState.Name;
```

If Current State is null, the log uses `None`. Otherwise, it uses the active state's name.

# Part 8: Implement Exit, Change, Enter

Find:

```csharp
// Exit the old state when one exists.
```

Add:

```csharp
if (CurrentState != null)
{
    CurrentState.Exit();
}
```

Find:

```csharp
// Change the current-state reference.
```

Add:

```csharp
CurrentState = nextState;
```

Find:

```csharp
// Log the transition.
```

Add:

```csharp
Day25_StateDebug.LogTransition(previousStateName, CurrentState.Name, reason);
```

Find:

```csharp
// Enter the new state.
```

Add:

```csharp
CurrentState.Enter();
```

The final lifecycle is:

```text
Exit old state
Change Current State
Log transition
Enter new state
```

## Compile and Test

1. Save all scripts.
2. Add `Day25_StateController` to an empty GameObject.
3. Enter Play Mode.
4. Confirm the Console shows:

```text
TRANSITION None -> Patrol: Initial state
ENTER Patrol: Following the patrol route and watching for the player.
```

# Part 9: Implement Patrol

Open:

```text
Day25/Starter/day25_patrol-state.cs
```

In `Enter()`, find:

```csharp
// Log the active Patrol behaviour.
```

Add:

```csharp
Day25_StateDebug.LogEnter(Name, "Following the patrol route and watching for the player.");
```

In `Update()`, find:

```csharp
// Transition to Chase when the player becomes visible.
```

Add:

```csharp
if (agent.IsPlayerVisible)
{
    agent.TransitionTo(agent.ChaseState, "Player became visible");
    return;
}
```

In `Exit()`, find:

```csharp
// Log that Patrol ended.
```

Add:

```csharp
Day25_StateDebug.LogExit(Name);
```

## Test Patrol to Chase

1. Enter Play Mode.
2. Confirm Current State begins as Patrol in the Console.
3. Turn `Player Visible` on in the Inspector.
4. A transition is requested, but Chase logging will not be complete until the next section.

# Part 10: Implement Chase

Open:

```text
Day25/Starter/day25_chase-state.cs
```

In `Enter()`, add below the TODO:

```csharp
Day25_StateDebug.LogEnter(Name, "Following the player using the agent's existing movement systems.");
```

In `Update()`, add:

```csharp
if (!agent.IsPlayerVisible)
{
    agent.TransitionTo(agent.SearchState, "Player was lost");
    return;
}
```

In `Exit()`, add:

```csharp
Day25_StateDebug.LogExit(Name);
```

## Compile and Test

1. Enter Play Mode with `Player Visible` off.
2. Turn `Player Visible` on.
3. Confirm Patrol exits and Chase enters.
4. Turn `Player Visible` off.
5. Confirm Chase exits and Search becomes Current State.

The immediate `return` ensures only one transition occurs during the old state's update, as required by the presentation. citeturn159search24L19-L19

# Part 11: Implement Search

Open:

```text
Day25/Starter/day25_search-state.cs
```

In `Enter()`, add:

```csharp
Day25_StateDebug.LogEnter(Name, "Investigating the player's last known area.");
```

In `Update()`, find:

```csharp
// Give Player Visible priority over Search Complete.
```

Add:

```csharp
if (agent.IsPlayerVisible)
{
    agent.TransitionTo(agent.ChaseState, "Player became visible during Search");
    return;
}

if (agent.IsSearchComplete)
{
    agent.TransitionTo(agent.PatrolState, "Search completed without finding the player");
    return;
}
```

In `Exit()`, add:

```csharp
Day25_StateDebug.LogExit(Name);
```

## Teaching Point

Transition conditions remain ordered rules. Player Visible is checked before Search Complete, so reacquiring the player wins if both values are true.

# Part 12: Run the Full Transition Test

Use the test sequence in `Documentation/day25-state-transition-table.md`.

1. Enter Play Mode with both values off. Confirm Patrol.
2. Turn `Player Visible` on. Confirm Chase.
3. Turn `Player Visible` off. Confirm Search.
4. Turn `Player Visible` on. Confirm Chase.
5. Turn `Player Visible` off. Confirm Search.
6. Leave `Player Visible` off and turn `Search Complete` on. Confirm Patrol.

For every transition, confirm the Console shows:

```text
EXIT old state
TRANSITION old -> new: reason
ENTER new state: behaviour
```

# Part 13: Verify the Diagram Still Matches

Return to the flowchart and transition table.

For each arrow, identify:

```text
Source state class
Condition inside Update()
TransitionTo() call
Destination state property
Console reason
```

# Troubleshooting

## No Initial Log

- Confirm `Day25_StateController` is attached to an active GameObject.
- Confirm `Start()` calls `TransitionTo(PatrolState, "Initial state")`.
- Confirm all states are created in `Awake()`.

## NullReferenceException During Start

- Confirm `PatrolState`, `ChaseState`, and `SearchState` are created before `Start()`.
- Confirm each state constructor stores `this.agent = agent`.

## State Re-enters Every Frame

- Confirm `TransitionTo()` rejects `nextState == CurrentState`.
- Confirm state transition conditions call `return` immediately after `TransitionTo()`.

## Search Immediately Returns to Patrol

- Turn `Search Complete` off before entering Search.
- Remember that Inspector booleans keep their current Play Mode values until manually changed.

## Wrong Transition Wins in Search

- Confirm `IsPlayerVisible` is checked before `IsSearchComplete`.
- Confirm each successful transition returns immediately.

## Console Fills Every Frame

- Do not place `Debug.Log` directly in state `Update()` unless diagnosing a specific issue.
- Use the supplied helper to log lifecycle events and transitions only.

# Additional Practice

Optional only:

1. Add an Attack state entered when `IsPlayerInAttackRange` is true during Chase.
2. Add a Flee state with a higher-priority critical-health transition.
3. Add a timer that resets in Search `Enter()` and updates in Search `Update()`.
4. Replace an Inspector Boolean with existing perception data from a previous agent.

# Completion Checklist

- Base state stores the controller reference.
- Three concrete state instances exist.
- Controller owns one Current State reference.
- Initial state enters correctly.
- Only Current State updates.
- `TransitionTo()` rejects null and accidental re-entry.
- Exit runs before changing state.
- Enter runs after changing state.
- Transition logs include old state, new state, and reason.
- Patrol transitions to Chase.
- Chase transitions to Search.
- Search transitions to Chase or Patrol.
- Search transition order is explicit.
- Successful transitions return immediately.
- Console output matches the state/transition table.
- Flowchart still matches the code.
