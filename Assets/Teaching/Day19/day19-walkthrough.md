# Day 19 Walkthrough: Implementing A*

## Demo Purpose
Implement A* from start to finish by coding node evaluation with G, H, and F, managing Open and Closed collections, selecting the next node, evaluating neighbours, and reconstructing the final path.

## Required Walkthrough Time
Target: approximately 60 minutes of the 70-minute practical block. The remaining time is reserved for compilation, explanation, questions, and debugging.

## Package Structure
- `Day19/Core`: supplied grid and demo controller
- `Day19/Starter`: the two files edited live
- `Day19/Teaching/Complete`: recovery and reference implementations

## Existing / Provided Functionality
`Day19_GridMap` builds a four-direction grid, supplies walkable neighbours, resets search state, and draws grid state with Gizmos. `Day19_DemoController` runs the search at startup. Review these responsibilities, but do not rebuild them live.

## Instructor Setup
1. Create a Unity scene named `Day_19_Implementing_AStar`.
2. Add an empty GameObject named `AStar Demo`.
3. Add `Day19_GridMap` and `Day19_DemoController`.
4. Expand **Blocked Coordinates** and add a short wall that does not completely divide the grid, for example `(4,1)`, `(4,2)`, `(4,3)`, `(4,4)`.
5. Enable Gizmos in the Scene view.
6. Enter Play Mode once. The untouched Starter should compile and report `No path found.`

# Part 1: Store the F Cost

Open: `Day19_GridNode.cs`  
Locate: `#region Public Methods`  
Locate method: `CalculateFCost()`  
Find: `// TODO 1: Calculate F from G and H.`  
Replace the line `FCost = 0;` directly below it with:

```csharp
FCost = GCost + HCost;
```

Recall: G is discovered cost so far, H is estimated cost remaining, and F is their combined comparison value.

# Part 2: Initialize the Search

Open: `Day19_AStarPathfinder.cs`  
Locate method: `FindPath(...)`  
Find: `// TODO 2: Initialize and add the start node to the Open List.`  
Add directly below:

```csharp
startNode.GCost = 0;
startNode.HCost = CalculateDistance(startNode, goalNode);
startNode.CalculateFCost();
startNode.IsOpen = true;
openNodes.Add(startNode);
```

The Open List now represents discovered nodes that remain available for exploration.

## Compile Checkpoint
Save both Starter scripts. Return to Unity and confirm the project compiles without errors.

# Part 3: Build the Search Loop

In `FindPath(...)`, find:

```csharp
// TODO 3: Repeat while the Open List contains nodes.
while (false)
```

Replace `while (false)` with:

```csharp
while (openNodes.Count > 0)
```

The search continues only while a possible route remains.

# Part 4: Select the Best Open Node

Locate method: `GetLowestCostNode(...)`  
Find: `// TODO 8: Compare Open nodes and return the best candidate.`  
Replace the existing return statement below it with:

```csharp
Day19_GridNode bestNode = openNodes[0];

foreach (Day19_GridNode candidateNode in openNodes)
{
    if (candidateNode.FCost < bestNode.FCost || candidateNode.FCost == bestNode.FCost && candidateNode.HCost < bestNode.HCost)
    {
        bestNode = candidateNode;
    }
}

return bestNode;
```

Then return to `FindPath(...)`. Find TODO 4 and replace:

```csharp
Day19_GridNode currentNode = null;
```

with:

```csharp
Day19_GridNode currentNode = GetLowestCostNode(openNodes);
```

The H comparison makes selection deterministic when two nodes have equal F values.

# Part 5: Finish or Close the Current Node

In `FindPath(...)`, find TODO 5. Add directly below:

```csharp
if (currentNode == goalNode)
{
    return ReconstructPath(goalNode);
}
```

Find TODO 6. Add directly below:

```csharp
openNodes.Remove(currentNode);
currentNode.IsOpen = false;
currentNode.IsClosed = true;
closedNodes.Add(currentNode);
```

Explain that Open means discovered but not fully explored. Closed means exploration of that node is complete.

## Compile Checkpoint
Save the script. Return to Unity and confirm compilation. Enter Play Mode. A path is not expected yet because neighbours are not evaluated.

# Part 6: Calculate Grid Distance

Locate method: `CalculateDistance(...)`  
Find TODO 9 and replace the existing return with:

```csharp
int horizontalDistance = Mathf.Abs(firstNode.Coordinate.x - secondNode.Coordinate.x);
int verticalDistance = Mathf.Abs(firstNode.Coordinate.y - secondNode.Coordinate.y);
return horizontalDistance + verticalDistance;
```

This is Manhattan distance, matching the supplied grid's up, down, left, and right connections.

# Part 7: Evaluate Neighbours

Return to `FindPath(...)`. Find TODO 7. Add directly below:

```csharp
List<Day19_GridNode> neighbours = gridMap.GetNeighbours(currentNode);

foreach (Day19_GridNode neighbourNode in neighbours)
{
    if (neighbourNode.IsClosed)
    {
        continue;
    }

    int candidateGCost = currentNode.GCost + CalculateDistance(currentNode, neighbourNode);

    if (candidateGCost < neighbourNode.GCost)
    {
        neighbourNode.Parent = currentNode;
        neighbourNode.GCost = candidateGCost;
        neighbourNode.HCost = CalculateDistance(neighbourNode, goalNode);
        neighbourNode.CalculateFCost();
    }

    if (!neighbourNode.IsOpen)
    {
        neighbourNode.IsOpen = true;
        openNodes.Add(neighbourNode);
    }
}
```

Pause at the candidate comparison. A node is updated only when the newly discovered route is cheaper than its currently recorded route. Updating the Parent stores the breadcrumb used during reconstruction.

## Focused Test
1. Save and compile.
2. Enter Play Mode.
3. Confirm yellow Open nodes and orange Closed nodes appear during/after the search.
4. The Console may still report `No path found` until reconstruction is implemented.

# Part 8: Reconstruct the Path

Locate method: `ReconstructPath(...)`  
Find TODO 10 and replace the existing return with:

```csharp
List<Day19_GridNode> path = new List<Day19_GridNode>();
Day19_GridNode currentNode = goalNode;

while (currentNode != null)
{
    path.Add(currentNode);
    currentNode = currentNode.Parent;
}

path.Reverse();
return path;
```

The Parent links run goal-to-start, so the list is reversed before it is returned.

## Final Test
1. Save and return to Unity.
2. Confirm compilation succeeds.
3. Enter Play Mode.
4. Confirm the Console reports a path with more than zero nodes.
5. Confirm cyan nodes connect the green Start to the red Goal without crossing the black blocked cells.
6. Stop Play Mode, add another blocked coordinate directly on the cyan path, and run again.
7. Confirm A* finds a different route.
8. Create a complete wall across the grid and confirm the Console reports `No path found.` Remove the wall after the test.

# Troubleshooting

## Project does not compile
- Confirm Core and Starter folders are both inside the same Unity `Assets` tree.
- Confirm each code block was inserted inside the named method and not below the class closing brace.
- Confirm there is only one runtime copy of each `Day19_` class.

## Search stops immediately
- Confirm the Start node sets `GCost` to `0` and is added to `openNodes`.
- Confirm the loop condition is `openNodes.Count > 0`.

## Open nodes appear but no progress is made
- Confirm the selected current node is removed from Open and marked Closed.
- Confirm walkable neighbours are added when `IsOpen` is false.

## Path is empty after reaching the goal
- Confirm cheaper routes assign `neighbourNode.Parent = currentNode`.
- Confirm reconstruction begins at the Goal and follows Parent until null.
- Confirm `path.Reverse()` runs before returning.

## Path crosses blocked cells
- Confirm `Day19_GridMap.GetNeighbours` only adds nodes where `IsWalkable` is true.
- Confirm blocked coordinates are within the grid bounds.

# Additional Practice
- Change the tie-breaker so equal F values preserve discovery order, then compare the explored area.
- Add weighted terrain by supplying a movement cost per node and including it in `candidateGCost`.
- Add diagonal neighbours and replace Manhattan distance with a matching heuristic. This is optional and should not be included in the required live demonstration.

# Completion Checklist
- [ ] F is calculated from G and H.
- [ ] Start is initialized and placed in Open.
- [ ] The search repeats while Open contains nodes.
- [ ] The lowest F node is selected, with H used as a tie-breaker.
- [ ] Selected nodes move from Open to Closed.
- [ ] Each walkable neighbour receives candidate G, H, F, and Parent updates.
- [ ] Goal-to-start Parent links are reversed into a start-to-goal path.
- [ ] A valid route, changed route, and impossible route have each been tested.
