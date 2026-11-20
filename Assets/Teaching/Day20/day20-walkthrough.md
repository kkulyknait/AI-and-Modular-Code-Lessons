# Day 20 Walkthrough: Path Grid Resolution

## Demo Purpose
Use the completed A* search from Day 19 to visualize how the same environment can be represented at different grid resolutions. Students do not code during this demonstration. They predict, observe, compare, and discuss.

## Learning Focus
- A* requires a searchable representation of space.
- A grid turns world space into searchable nodes.
- Nodes can be walkable or non-walkable.
- Increasing resolution increases the number of nodes that must be represented and potentially searched.
- The underlying environment remains unchanged while its searchable representation becomes denser.

## Package Structure
- `Assets/Scenes/Day_20_Scene`: the finished demo scene, ready to open.
- `Assets/Scripts/Core/Day20_GridNode.cs`: stable node data used by the demonstration.
- `Assets/Teaching/Day20/Complete/Day20_GridMap.cs`: builds and subdivides the grid.
- `Assets/Teaching/Day20/Complete/Day20_AStarPathfinder.cs`: completed A* search with node counters.
- `Assets/Teaching/Day20/Complete/Day20_DemoController.cs`: runs searches and times them.
- `Assets/Teaching/Day20/Complete/Day20_DemoUI.cs`: Play Mode controls and statistics.

There is no Starter folder. No live student coding is required.

## Setup
The package includes a finished scene. Open `Assets/Scenes/Day_20_Scene`, enable Gizmos in the Scene view, enter Play Mode, and confirm a path appears at resolution 1. If Unity asks to import TMP Essentials, accept.

To build the scene from scratch instead:
1. Create a new Unity scene named `Day_20_Path_Grid_Resolution`.
2. Create an empty GameObject named `Path Grid Demo`.
3. Add `Day20_GridMap` and `Day20_DemoController` to it.
4. In `Day20_GridMap`, keep:
   - Base Width: `10`
   - Base Height: `7`
   - Base Cell Size: `1`
   - Base Start Coordinate: `(1, 1)`
   - Base Goal Coordinate: `(8, 5)`
   - Starting Resolution: `1`
   - Maximum Resolution: `8` (the finished scene is already set to 8)
5. Configure a small partial obstacle wall such as `(4,1)`, `(4,2)`, `(4,3)`, `(4,4)`.
6. Create a Canvas with:
   - one TextMeshPro text object for statistics (GameObject > UI > Text - TextMeshPro);
   - one UI `Button` labelled `Increase Resolution`.
7. Add `Day20_DemoUI` to the Canvas.
8. Assign the Button and TextMeshPro text references. The controller can be assigned manually or found automatically at runtime.
9. Enable Gizmos in the Scene view.
10. Enter Play Mode and confirm a path appears at resolution 1.

# Part 1: Re-establish the Day 19 Search

Begin at resolution 1. Point out that the visible grid still represents the same 10 by 7 source space used to explain A*.

Ask:
- What does each cube represent?
- Which information does A* actually search?
- Which parts of the display represent walkability rather than movement?

Reinforce that A* searches the grid data. It does not understand the visual environment directly.

# Part 2: Source Grid vs Resolved Grid

Explain that Day 20 separates two ideas:

- **Source grid:** the original 10 by 7 description of the environment.
- **Resolved grid:** the searchable nodes produced after subdivision.

At resolution 1, every source cell creates one searchable node.

Use the statistics display to identify:
- resolution;
- grid dimensions;
- total nodes;
- walkable nodes;
- path nodes;
- opened and closed nodes;
- elapsed search time.

Emphasize that the source environment will not change during the comparison.

# Part 3: Resolution 1 Baseline

Record or verbally note the resolution 1 statistics.

Before changing anything, ask students to predict which values must change when each cell becomes a 2 by 2 group of nodes.

Expected observation:
- width doubles;
- height doubles;
- total node count becomes four times larger;
- blocked source cells remain completely blocked after subdivision;
- start and goal move to the lowest-left child of their original source cells.

# Part 4: Increase to Resolution 2

Click `Increase Resolution` once.

The demonstration automatically:
1. changes resolution from 1 to 2;
2. rebuilds the searchable grid;
3. subdivides every source cell into 2 by 2 child nodes;
4. preserves source-cell walkability across all children;
5. resets the previous search;
6. reruns A*;
7. refreshes the statistics.

Compare the new 20 by 14 grid with the previous 10 by 7 grid.

Ask:
- Did the environment itself change?
- Why did the total node count rise much faster than either width or height?
- Did A* need to know that subdivision occurred, or did it simply search the new nodes?

# Part 5: Increase to Resolution 4

Before clicking again, ask students to predict the new dimensions and node count.

Click `Increase Resolution`.

Expected dimensions:
- 40 by 28;
- 1,120 total nodes.

Compare:
- total nodes;
- opened nodes;
- closed nodes;
- path nodes;
- search time.

Do not present one timing measurement as a benchmark. The important observation is the growth in searchable data. Timing is included to make the performance discussion concrete, but individual runs can vary.

# Part 6: Increase to Resolution 8

Ask students for one final prediction before clicking.

Click `Increase Resolution`.

Expected dimensions:
- 80 by 56;
- 4,480 total nodes.

The button disables at the configured maximum resolution.

Compare resolution 8 to resolution 1. The same 10 by 7 source environment is now represented by 64 times as many searchable nodes.

Typical results for the finished scene (your timings will differ):

| Resolution | Grid | Total nodes | Walkable | Path nodes | Opened | Closed |
| --- | --- | --- | --- | --- | --- | --- |
| 1 | 10 x 7 | 70 | 66 | 12 | 33 | 11 |
| 2 | 20 x 14 | 280 | 264 | 23 | 65 | 22 |
| 4 | 40 x 28 | 1,120 | 1,056 | 45 | 129 | 44 |
| 8 | 80 x 56 | 4,480 | 4,224 | 89 | 257 | 88 |

Total nodes grow four times with each step. The nodes A* actually opens grow about twice. A* only looks at what it needs, but the whole grid still has to be built and stored.

# Part 7: Wrap-Up Discussion

Use the four runs to separate these concepts:

- **Environment:** the space the game is representing.
- **Source grid:** the original walkable/non-walkable description.
- **Resolution:** how finely each source cell is subdivided.
- **Search grid:** the actual nodes A* evaluates.
- **Search cost:** affected by how many nodes may need to be considered.

Closing question:

> If increasing resolution adds substantially more searchable data, when is that additional detail worth paying for?

The demo intentionally does not attempt to prove that the higher-resolution path is more accurate. The purpose is to visualize representation and computational trade-offs.

# Troubleshooting

## Button does nothing
- Confirm the Button reference is assigned on `Day20_DemoUI`.
- Confirm an active `Day20_DemoController` exists in the scene.
- Confirm the current resolution is below Maximum Resolution.

## Statistics do not appear
- Confirm the TextMeshPro text reference is assigned.
- If Unity prompts for TMP Essentials, import them (Window > TextMeshPro > Import TMP Essential Resources).
- Confirm the Canvas and Text GameObject are active.

## Grid does not appear
- Enable Gizmos in the Scene view.
- Confirm `Day20_GridMap` and `Day20_DemoController` are on the same GameObject.

## Path crosses blocked nodes
- Confirm blocked coordinates describe source-grid coordinates, not resolved coordinates.
- Confirm the source Start and Goal cells are not blocked.

## Nodes look offset from the scene
- Node positions are the centre of each cell, so a grid at the origin covers 0 to 10 on X and 0 to 7 on Z.

## Timing appears inconsistent
- This is expected for very short searches. Compare node counts first. Treat milliseconds as observational rather than a deterministic benchmark.

# Completion Checklist
- [ ] Resolution changes during Play Mode through the UI button.
- [ ] Resolution progresses 1 to 2 to 4 to 8.
- [ ] Source walkability is inherited uniformly by child nodes.
- [ ] Start and Goal map to the lowest-left child.
- [ ] Increasing resolution rebuilds and resets the searchable grid.
- [ ] A* reruns automatically after each resolution change.
- [ ] Statistics show grid size, node counts, path nodes, explored nodes, and elapsed time.
- [ ] No student coding is required.
