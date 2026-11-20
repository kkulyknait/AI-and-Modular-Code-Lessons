# Day 21 Walkthrough: Implementing Path Grids

## Demo Purpose

Day 21 connects the A* search from earlier lessons to a real Unity environment. The supplied dungeon already contains an authored node grid so class time is spent on the information A* needs rather than manually placing dozens of nodes or learning Unity Editor tooling.

The Human from the asset pack represents the Start position. The Chest from the asset pack represents the Goal position.

```text
Human World Position → Start Node
Chest World Position → Goal Node
Scene Geometry → Walkability
Start + Goal + Walkability → A* → Path
```

The course outcome for Module 4 is to develop, test, and refine pathfinding and movement rules. Day 21 therefore focuses on preparing searchable grid data in-engine rather than building a separate grid-authoring tool.

# Part 1: Inspect the Authored Grid

Open the supplied Day 21 scene and select several node objects.

Review:

```text
Coordinate
World Position
Is Walkable
G / H / F
Open / Closed
Parent
```

Move the Scene view around the dungeon with Gizmos enabled. The grid should overlay the dungeon without hiding the environment.

## Discussion

The node data has not fundamentally changed from the earlier A* demos. Day 21 gives each node a deliberate relationship to Unity world space.

## Testing Checkpoint

Select nodes at several grid coordinates and compare their Transform positions. Confirm the node list is ordered by row so `GetNode()` can use `gridY * gridWidth + gridX`.

# Part 2: Review the Supplied Path Grid

Open `Day21_PathGrid.cs` (Assets/Scripts/Starter).

The grid is a 9 x 9 grid of 1-unit cells. The Obstacles object holds three difficulty groups (Easy, Medium, Hard). Easy is active; Medium and Hard are switched off. Turn on only one group at a time. Easy plus Medium together seal the Chest in.

The Starter already supplies:

- grid dimensions
- node size
- grid origin
- Human Transform
- Chest Transform
- obstacle LayerMask
- authored node collection
- neighbour lookup
- A* reset
- path storage
- Day 20-style translucent Gizmos

- an `Awake()` that calls `RefreshWalkability()` when Play mode starts (the right-click commands do not run Awake, so refresh by hand in the editor)
- right-click menu commands on the Path Grid component: Refresh Walkability, Run Search, and Check Grid To World
- neighbour order Up, Right, Down, Left (the order used in Days 19 and 20)

Students live-code:

```text
GridToWorld
WorldToNode
RefreshWalkability
```

# Part 3: Implement Grid to World

Locate `GridToWorld(int gridX, int gridY)`.

### Step 1: Grid Offset

Under:

```csharp
// Calculate the X/Z offset from the grid origin.
```

add:

```csharp
Vector3 gridOffset = new Vector3(gridX * nodeSize, 0f, gridY * nodeSize);
```

### Step 2: Cell Centre

Under:

```csharp
// Calculate the half-cell centre offset.
```

add:

```csharp
Vector3 centreOffset = new Vector3(nodeSize * 0.5f, 0f, nodeSize * 0.5f);
```

### Step 3: Return World Position

Under:

```csharp
// Combine origin, offset, and centre offset.
```

replace the placeholder return with:

```csharp
return gridOrigin.position + gridOffset + centreOffset;
```

## Discussion

```text
World Position = Origin + Grid Offset + Cell Centre Offset
```

Unity X represents the grid column. Unity Z represents the grid row.

## Compile Checkpoint

Save and compile.

## Testing Checkpoint

Predict and compare the world position of `(0,0)`, `(1,0)`, `(0,1)`, and one interior coordinate against existing node Transforms.

Then right-click the Path Grid component and choose **Check Grid To World**. The Console should read `GridToWorld matches all 81 nodes.`

# Part 4: Implement World to Node

Locate `WorldToNode(Vector3 worldPosition)`.

### Step 1: Relative Position

Under:

```csharp
// Express the world position relative to the grid origin.
```

add:

```csharp
Vector3 relativePosition = worldPosition - gridOrigin.position;
```

### Step 2: Convert to Coordinates

Under:

```csharp
// Convert the relative X/Z position into grid coordinates.
```

add:

```csharp
int gridX = Mathf.FloorToInt(relativePosition.x / nodeSize);
int gridY = Mathf.FloorToInt(relativePosition.z / nodeSize);
```

### Step 3: Validate Bounds

Under:

```csharp
// Reject coordinates outside the grid.
```

add:

```csharp
if (gridX < 0 || gridX >= gridWidth || gridY < 0 || gridY >= gridHeight)
{
    return null;
}
```

### Step 4: Retrieve the Node

Under:

```csharp
// Return the node at the valid coordinate.
```

replace the placeholder return with:

```csharp
return GetNode(gridX, gridY);
```

## Discussion

This reverse mapping is what lets world-space gameplay objects become A* inputs.

## Compile Checkpoint

Save and compile.

# Part 5: Determine the Human Start Node

The supplied property is:

```csharp
public Day21_GridNode StartNode
{
    get
    {
        if (humanTransform == null)
        {
            return null;
        }

        return WorldToNode(humanTransform.position);
    }
}
```

Review only.

## Testing Checkpoint

Move the Human to several traversable cells. The translucent green Gizmo should change to the node containing the Human.

Ask students to predict the node before moving the Human.

# Part 6: Determine the Chest Goal Node

The supplied property is:

```csharp
public Day21_GridNode GoalNode
{
    get
    {
        if (chestTransform == null)
        {
            return null;
        }

        return WorldToNode(chestTransform.position);
    }
}
```

Review only.

## Testing Checkpoint

Move the Chest to several traversable cells. Confirm the translucent red Goal node follows the Chest.

Move the Chest outside the grid and confirm GoalNode becomes null rather than producing an invalid list index.

# Part 7: Implement Physics-Based Walkability

Locate `RefreshWalkability()`.

### Step 1: Check Volume

Under:

```csharp
// Calculate the CheckBox half extents once.
```

add:

```csharp
Vector3 halfExtents = new Vector3(nodeSize * 0.45f, 0.5f, nodeSize * 0.45f);
```

### Step 2: Evaluate Nodes

Under:

```csharp
// Evaluate every authored node against the Obstacle layer.
```

add:

```csharp
foreach (Day21_GridNode node in nodes)
{
    if (node == null)
    {
        continue;
    }

    bool isBlocked = Physics.CheckBox(node.WorldPosition, halfExtents, Quaternion.identity, obstacleMask, QueryTriggerInteraction.Ignore);
    node.IsWalkable = !isBlocked;
}
```

## Discussion

The physics query is translating scene geometry into navigation data.

```text
Obstacle overlap → IsWalkable false
No obstacle overlap → IsWalkable true
```

## Compile Checkpoint

Save and compile.

## Testing Checkpoint

Right-click the Path Grid component and choose **Refresh Walkability**. (Pressing Play also refreshes it, because Awake calls it.) Confirm open nodes remain light/translucent and nodes overlapping dungeon walls become dark/translucent.

# Part 8: Walkability Does Not Have to Come from Physics

Pause the code demo.

Physics is one way to establish walkability, not the definition of walkability.

A designer could manually set an open node to:

```text
Is Walkable = false
```

for reasons such as:

- undesirable shortcut
- reserved gameplay space
- unsafe region
- NPC restriction
- level-design boundary

A* ultimately consumes `IsWalkable`. It does not care whether that data came from physics, manual authoring, generated data, or a combination.

## Testing Checkpoint

Refresh walkability first. Then choose one physically open node, untick its Is Walkable box, and verify its Gizmo changes to blocked.

Do not refresh walkability again until you have run the search, and do not press Play first: Refresh Walkability (and Play, through Awake) recalculates every node from the physics and overwrites the manual choice. In a real project the manual block would be stored in a separate flag.

# Part 9: Run A* from Human to Chest

The supplied method is:

```csharp
public void RunSearch()
{
    Day21_AStarPathfinder pathfinder = new Day21_AStarPathfinder(this);
    SetPath(pathfinder.FindPath(StartNode, GoalNode));
}
```

Review only.

The important point is that A* itself did not need a new conceptual algorithm.

The input changed from manually selected coordinates to nodes derived from actual world objects.

```text
Human.position → StartNode
Chest.position → GoalNode
```

## Testing Checkpoint

Right-click the Path Grid component and choose **Run Search**. Confirm the cyan path begins at the green Human node and ends at the red Chest node.

# Part 10: Move the Human

1. Move the Human to another traversable area.
2. Predict the new Start node.
3. Run the search again.

Expected:

```text
Green node changes
Cyan path changes
Red node remains at Chest
```

# Part 11: Move the Chest

1. Move the Chest to another traversable area.
2. Predict the new Goal node.
3. Run the search again.

Expected:

```text
Red node changes
Cyan path changes
Green node remains at Human
```

# Part 12: Change Walkability

Use two tests.

## Physics Test

Move an obstacle onto a previously open node, choose Refresh Walkability, then Run Search.

Expected:

```text
Node becomes blocked
A* routes around it
```

## Manual Test

Refresh walkability, untick Is Walkable on a physically open node on the current path, then Run Search without refreshing.

Expected:

```text
A* respects the designer restriction
```

This confirms that the pathfinder consumes navigation data rather than repeatedly reasoning about scene colliders.

# Part 13: Visualization Review

The Gizmo system deliberately reuses the Day 20 state model.

Visual priority is:

```text
Start
Goal
Path
Closed
Open
Walkability
```

Therefore, a Start node on the current path remains green rather than cyan.

The colours are translucent so students can inspect dungeon geometry underneath the navigation grid.

If Scene-view filled cubes are still too visually heavy on a particular machine, reduce the alpha values or switch ordinary Walkable nodes to `Gizmos.DrawWireCube` while keeping special states filled.

# Part 14: Compare Grid Workflows

Return to the three approaches from the presentation.

## Manual

Designer authors navigation data directly.

## Editor-Generated

A development tool creates candidate data and the designer reviews it.

## Runtime-Generated

The game creates navigation data while running.

## Our Hybrid

```text
Grid placement → supplied/authored
Grid ↔ World conversion → implemented in class
Walkability → physics sampled
Walkability override → manual option
Start → Human position
Goal → Chest position
Search → existing A*
Visualization → reused Day 20 Gizmos
```

# Troubleshooting

## Green Start Node Is Wrong

- confirm Human Transform reference
- confirm grid origin
- confirm node size
- confirm X/Z conversion

## Red Goal Node Is Wrong

- confirm Chest Transform reference
- confirm WorldToNode bounds
- confirm Chest is physically inside the authored grid

## Grid Is Offset

- confirm grid origin represents the lower-left corner used by the conversion formula
- confirm authored node centres use the same half-cell offset
- confirm nodeSize matches authored spacing

## Walls Remain Walkable

- confirm wall colliders use the Obstacle layer
- confirm Obstacle is included in obstacleMask
- confirm CheckBox extents overlap the wall

## Open Floor Becomes Blocked

- confirm Floor is not in obstacleMask
- reduce X/Z CheckBox extents
- verify the Y check volume does not overlap unrelated dungeon geometry

## Path Is Empty

- confirm StartNode and GoalNode are not null
- confirm both are walkable
- confirm a walkable route exists
- confirm node ordering matches grid coordinates

## Gizmos Hide the Dungeon

- reduce alpha values
- verify Scene Gizmos are enabled
- optionally use wire cubes for ordinary walkable cells

# Additional Practice

1. Replace Physics.CheckBox with Physics.CheckSphere and compare results.
2. Reduce node size and discuss precision versus node count.
3. Increase node size and discuss loss of navigation detail.
4. Add a second manually restricted region.
5. Write pseudocode for automatically generating the supplied node grid.
6. Add a different goal object and reuse WorldToNode without changing A*.

# Completion Checklist

- supplied dungeon/grid scene opens
- Human determines Start dynamically
- Chest determines Goal dynamically
- GridToWorld produces expected node centres
- WorldToNode returns expected nodes
- outside-grid positions return null
- physics can refresh walkability
- manual walkability restriction is demonstrated
- Start is translucent green
- Goal is translucent red
- path is translucent cyan
- blocked nodes remain visible without hiding the dungeon
- A* routes from Human to Chest
- moving Human changes Start and path
- moving Chest changes Goal and path
- changing walkability changes path
- students can distinguish manual, editor-generated, runtime-generated, and hybrid grid workflows
