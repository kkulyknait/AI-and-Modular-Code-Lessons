# Day 22 Walkthrough: Following A* Paths with Contextual Movement

## Demo Purpose

Day 22 completes the integration required by the lesson objectives: agents follow sequences of path nodes, A* is integrated with contextual movement, and the Human navigates toward a changing goal. 

```text
Human → Start Node
Chest → Goal Node
A* → Path
Current Path Node → Desired Direction
Context Sensor → Available Local Directions
Selected Direction → Rigidbody Movement
```

The course design expects students to identify desired movement direction, develop movement rules that aid path following, and test/revise those rules.

## Supplied Functionality

Students live-code only `Day22_PathfindingAgent.cs`.

Supplied complete:

- `Day22_GridNode.cs`
- `Day22_PathGrid.cs`
- `Day22_AStarPathfinder.cs`
- `Day22_AgentContext.cs`
- `Day22_ContextSensor.cs`
- configured Human prefab
- configured Chest Goal
- Chair prefab (`Assets/Prefabs/Day22/Chair.prefab`), placed in the scene and switched off
- completed dungeon/grid scene

# Setup Check

Before class:

- Open **Edit > Project Settings > Tags and Layers**. Layer 6 should be `Obstacle` (used since Day 21). Name layer 7 `ContextObstacle` if it is empty.
- The walls are on layer 6. The Chair prefab is on layer 7. The Human is on Default.
- Path Grid **Obstacle Mask** is layer 6 only. Context Sensor **Blocking Mask** is layers 6 and 7.
- Path Grid neighbours are checked in Up, Right, Down, Left order, matching Days 19 to 21.
- Context Sensor starts with Detection Distance `0.4` and Sensor Radius `0.2`. The sensor reaches distance plus radius from the Human, which must stay under the 0.5 gap between a node centre and the wall behind it. Larger values stop the Human short of cells that touch a wall.

# Part 1: Verify the Starting State

Open the supplied Day 22 scene.

Before editing `Day22_PathfindingAgent.cs`, enter Play Mode.

Expected:

```text
Green Start node under the Human
Red Goal node under the Chest
Permanent walkability visible (2 dark nodes, at (6,8) and (8,6))
No cyan path
Human stationary
Chair not visible
```

This baseline is intentional. Day 21 prepared the searchable grid. Day 22 begins by requesting and then following a path.

## Discussion

Pathfinding produces a sequence toward a goal. Movement consumes that sequence. Day 17 explicitly distinguished pathfinding from the means of moving through the path.

# Part 2: Review the Supplied Framework

## Path Grid

`Day22_PathGrid` owns:

```text
Start from Human root Transform
Goal from Chest Transform
walkability
on-demand neighbours
A* path storage
Gizmos
```

## A*

`Day22_AStarPathfinder` is attached as a Unity component and assigned to the Path Grid. `RunSearch()` calls its `FindPath()` method.

## Context Sensor

The sensor stores local directions:

```text
Human forward
Human right
Human backward
Human left
```

These directions rotate with the Human. They are not fixed grid directions.

The sensor is explicitly refreshed by the Pathfinding Agent before movement decisions. This gives a deterministic sequence:

```text
Sense → Decide → Move
```

# Part 3: Request the First Path

Open:

```text
Day22_PathfindingAgent.cs
```

Locate `Start()`.

Under:

```csharp
// Request the first path from the Human to the Chest.
```

add:

```csharp
RequestPath();
```

Locate `RequestPath()`.

Under:

```csharp
// Run A* and store the generated path in the agent context.
```

add:

```csharp
List<Day22_GridNode> path = pathGrid.RunSearch();
context.SetPath(path, pathGrid.GoalNode);
```

## Compile Checkpoint

Save and compile.

## Testing Checkpoint

Enter Play Mode.

Expected:

```text
Cyan path appears
Human remains stationary because movement is not complete yet
```

# Part 4: Sense Before Deciding

Locate `FixedUpdate()`.

Under:

```csharp
// Sense before deciding how to move.
```

add:

```csharp
contextSensor.RefreshAvailableDirections();
```

This deliberately avoids coupling two separate `FixedUpdate()` methods whose execution order would otherwise be ambiguous.

# Part 5: Detect Goal Changes

Under:

```csharp
// Request another path when the Chest enters a different Goal node.
```

add:

```csharp
if (pathGrid.GoalNode != context.PreviousGoalNode)
{
    RequestPath();
}
```

The path recalculates when the Chest changes Goal nodes, not every frame.

# Part 6: Validate the Current Path

Under:

```csharp
// Stop when no usable path exists.
```

add:

```csharp
if (context.CurrentPath.Count == 0 || context.CurrentPathIndex >= context.CurrentPath.Count)
{
    return;
}
```

`Day22_AgentContext.SetPath()` starts the index at `1` when possible because index `0` is the Start node the Human already occupies.

# Part 7: Advance Through the Path

Under:

```csharp
// Advance after reaching the current path node.
```

add:

```csharp
Day22_GridNode currentPathNode = context.CurrentPath[context.CurrentPathIndex];
Vector3 directionToNode = currentPathNode.WorldPosition - rigidBody.position;
directionToNode.y = 0f;

if (directionToNode.magnitude <= nodeReachDistance)
{
    context.CurrentPathIndex++;
    context.ClearCommittedDirection();
}
```

Under:

```csharp
// Stop after reaching the end of the path.
```

add:

```csharp
if (context.CurrentPathIndex >= context.CurrentPath.Count)
{
    return;
}
```

# Part 8: Calculate the Desired Path Direction

Under:

```csharp
// Calculate the desired direction toward the current path node.
```

add:

```csharp
currentPathNode = context.CurrentPath[context.CurrentPathIndex];
Vector3 desiredDirection = currentPathNode.WorldPosition - rigidBody.position;
desiredDirection.y = 0f;
```

This is the long-range A* intent.

# Part 9: Handle No Local Direction

Locate `SelectBestAvailableDirection()`.

Under:

```csharp
// Stop when no local directions are available.
```

add:

```csharp
if (context.AvailableDirections.Count == 0)
{
    context.ClearCommittedDirection();
    return Vector3.zero;
}

Vector3 normalizedDesiredDirection = desiredDirection.normalized;
```

# Part 10: Preserve Useful Direction Commitment

Under:

```csharp
// Keep a useful committed direction.
```

add:

```csharp
if (context.HasCommittedDirection && IsDirectionAvailable(context.CurrentDirection))
{
    if (Vector3.Dot(context.CurrentDirection, normalizedDesiredDirection) >= 0f)
    {
        return context.CurrentDirection;
    }
}

Vector3 bestDirection = Vector3.zero;
float bestAlignment = float.NegativeInfinity;
```

The `>= 0f` threshold permits a perpendicular committed direction to persist temporarily. This helps the Human complete a local detour without instantly oscillating back toward the path. If classroom testing shows excessive sideways travel, tighten this value later rather than adding new architecture during the demo.

# Part 11: Choose the Best Local Direction

Under:

```csharp
// Compare each available direction against the desired path direction.
```

add:

```csharp
foreach (Vector3 candidateDirection in context.AvailableDirections)
{
    if (IsReverseDirection(candidateDirection))
    {
        continue;
    }

    float alignment = Vector3.Dot(candidateDirection.normalized, normalizedDesiredDirection);

    if (alignment <= bestAlignment)
    {
        continue;
    }

    bestAlignment = alignment;
    bestDirection = candidateDirection.normalized;
}
```

This reuses the Day 15 dot-product movement selection with a new desired direction: the next A* node. Day 15 focused on following targets over time while adapting to contextual conditions.

# Part 12: Permit Dead-End Reversal

Under:

```csharp
// Permit reversal when no other direction is available.
```

add:

```csharp
if (bestDirection.sqrMagnitude <= 0f)
{
    foreach (Vector3 candidateDirection in context.AvailableDirections)
    {
        float alignment = Vector3.Dot(candidateDirection.normalized, normalizedDesiredDirection);

        if (alignment <= bestAlignment)
        {
            continue;
        }

        bestAlignment = alignment;
        bestDirection = candidateDirection.normalized;
    }
}
```

# Part 13: Commit the Direction

Under:

```csharp
// Commit to the selected direction.
```

replace the placeholder return with:

```csharp
context.SetCommittedDirection(bestDirection);
return bestDirection;
```

## Compile Checkpoint

Save and compile.

# Part 14: Use the Selected Direction

Return to `FixedUpdate()`.

Under:

```csharp
// Select the available local direction that best supports the path.
```

add:

```csharp
Vector3 movementDirection = SelectBestAvailableDirection(desiredDirection);

if (movementDirection.sqrMagnitude <= 0f)
{
    return;
}
```

Under:

```csharp
// Rotate and move the Human.
```

add:

```csharp
RotateAgent(movementDirection);
MoveAgent(movementDirection);
```

## Compile Checkpoint

Save and compile.

# Part 15: Follow the Path Without Chairs

Enter Play Mode with the Chair still switched off.

Expected path (17 nodes, 16 steps): up the left edge to (0,7), across row 7 to (7,7), up to (7,8), then right to (8,8).

Confirm:

- cyan path generates;
- Human begins moving toward path index `1`;
- Human advances path nodes;
- Human reaches the Chest;
- movement stops at path completion.

# Part 16: Move the Chest

Move the Chest to a different walkable node while Play Mode is running.

Expected:

```text
Goal node changes
new A* path appears
path index resets
Human follows the replacement path
```

# Part 17: Add a Contextual Chair

Tick the Chair on in the Hierarchy. It sits at `(3, 0, 6.6)`, inside cell (3,7) but 0.4 units off the centre of that cell, so it partly blocks the route along row 7.

Do not refresh permanent grid walkability.

Place the Chair off-centre. A Chair exactly on a node centre makes that node unreachable, and the Human circles it (see Part 19).

Expected:

```text
A* may still route through the Chair cell
Context Sensor removes locally blocked directions
Human chooses the remaining direction that best supports the current path node
Human converges back toward the path after clearing the Chair
```

Contextual avoidance uses detection to adjust movement direction rather than waiting for a collision response, which is consistent with the earlier avoidance lesson.

# Part 18: Wall vs Chair

## Permanent Wall

```text
Layer: Obstacle
included in Path Grid walkability
A* plans around it
Context Sensor also sees it as a safety fallback
```

## Chair

```text
Layer: ContextObstacle
not included in Path Grid walkability
A* may plan through its cell
Context Sensor handles it locally
```

Ask:

> Which system owns this problem?

This comparison directly reinforces the course goal of integrating AI components into a larger system and refining their interactions.

# Part 19: Scope Boundary

If a Chair completely seals a one-cell corridor, contextual movement cannot create a route where no physical space exists.

The same applies to a Chair sitting exactly on a path node. The Human can never reach within Node Reach Distance of that node centre, so it circles the Chair and never advances.

Those cases require:

```text
dynamic walkability
repathing around the Chair
moving/removing the Chair
or wider navigation space
```

Those are outside the required Day 22 demo.

# Troubleshooting

## No Cyan Path

- confirm the Path Grid has a `Day22_AStarPathfinder` component reference;
- confirm Human root is assigned as `Human Transform`;
- confirm Chest root is assigned as `Chest Transform`;
- confirm Start and Goal nodes are walkable;
- confirm the row-major Nodes list has not been reordered.

## Human Does Not Move

- confirm `Day22_PathfindingAgent` references the Path Grid;
- confirm Human root has Rigidbody, CapsuleCollider, AgentContext, ContextSensor, and PathfindingAgent;
- confirm Human is not on `Obstacle` or `ContextObstacle`;
- confirm CurrentPath has more than one node;
- confirm ContextSensor returns at least one direction.

## Chair Is Ignored

- confirm Chair has a Collider;
- confirm Chair is switched on and uses layer 7 (`ContextObstacle`);
- confirm ContextSensor Blocking Mask includes `ContextObstacle`;
- increase Detection Distance if necessary.

## Chair Blocks A*

- remove `ContextObstacle` from Path Grid Obstacle Mask;
- confirm Chair is not on `Obstacle`.

## Human Stops Short Near a Wall

- Detection Distance plus Sensor Radius is too large. Keep it under 0.7, for example `0.4` and `0.2`.

## Human Circles the Chair

- the Chair is on a path node centre. Move it about 0.4 units off the centre.

## Human Oscillates

- reduce Move Speed;
- increase Node Reach Distance slightly;
- verify only `Day22_PathfindingAgent` moves the Rigidbody;
- keep Chairs out of one-cell bottlenecks;
- test a stricter committed-alignment threshold only if needed.

## Wrong Start Node

- ensure Path Grid references the moving Human root, not the art child;
- confirm Grid Origin and Node Size match the authored grid.

## Wrong Goal Node

- ensure Path Grid references the Chest root;
- confirm Chest remains inside grid bounds.

# Completion Checklist

- starting scene shows no path before live code;
- Start follows Human root;
- Goal follows Chest root;
- first path appears after `RequestPath()` is implemented;
- A* component is referenced rather than instantiated with `new`;
- Context Sensor refreshes before movement decisions;
- path index begins after the Start node;
- Human follows path nodes;
- moving Chest recalculates the path;
- permanent walls affect A*;
- Chairs do not affect grid walkability;
- Chairs affect local available directions (Chair switched on, off-centre);
- Human locally navigates around Chairs;
- Human resumes path following afterward;
- no legacy Input API is used;
- no Console errors remain.
