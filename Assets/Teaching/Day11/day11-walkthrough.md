# Day 11 Walkthrough: Collision Avoidance

## Demo purpose

Day 11 extends the contextual movement project from Day 10. The HumanAgent already stores a Chest target and moves directly toward it. In this demonstration, you will add a forward ray sensor, store avoidance context, temporarily override the goal direction, and visualize the sensor and response with `Debug.DrawRay` and Gizmos.

The main progression is:

```text
Day 10
Detect what was contacted or entered.

Day 11
Sense ahead and change direction before contact.
```

## Package structure

```text
day11-collision-avoidance/
├── day11-walkthrough.md
├── instructor-notes.md
└── Day11/
    ├── Core/
    │   └── AgentContext.cs
    ├── Starter/
    │   ├── AvoidanceSensor.cs
    │   └── SimpleAgentController.cs
    └── Complete/
        ├── AvoidanceSensor.cs
        └── SimpleAgentController.cs
```

Use the files from `Starter` during the guided demonstration. The `Complete` files are finished reference versions. Do not place Starter and Complete copies in the Unity project at the same time.

## Existing Day 10 requirements

Before beginning, the project should already contain:

- `DetectionType.cs`
- `DetectionSource.cs`
- `DetectionAgent.cs`
- `CameraFollow.cs`
- A HumanAgent with `AgentContext`, `Rigidbody`, and a target-seeking controller
- A Chest target
- A Spike Trap configured as `DetectionType.Hazard`
- Walls configured as `DetectionType.Obstacle`

## Part 1: Open the existing Module 3 project

1. Open Unity Hub.
2. Open the Module 3 project used for Day 10.
3. Allow Unity to compile.
4. Open `Day_10_Scene`.
5. Enter Play Mode and confirm the HumanAgent can move toward the Chest.
6. Confirm the Console has no errors.

### Discussion

The current movement rule is direct:

```text
IF a target exists
    Move toward the target
```

This goal does not yet account for a hazard or wall between the agent and its target.

### Checkpoint

- HumanAgent has a Rigidbody.
- `CurrentTarget` refers to the Chest target zone or another transform positioned at the Chest.
- The agent moves when Play Mode starts.
- Camera follow still works.

Exit Play Mode.

## Part 2: Import the Day 11 package

1. Import the supplied Day 11 `.unitypackage`, or copy the package files into the project.
2. Create `Assets/Scripts/Day11` if the package did not create it.
3. Copy `Day11/Core/AgentContext.cs` into `Assets/Scripts/Day11` and replace the Day 10 version.
4. Copy `Day11/Starter/AvoidanceSensor.cs` into `Assets/Scripts/Day11`.
5. Copy `Day11/Starter/SimpleAgentController.cs` into `Assets/Scripts/Day11` and replace the Day 10 version.
6. Keep `Day11/Complete` outside the Unity project.

### Compile checkpoint

Expected result:

- Unity compiles with no errors.
- `AgentContext` now exposes `IsAvoiding` and `AvoidanceDirection`.
- The existing HumanAgent references remain intact.

## Part 3: Create the Day 11 scene

1. Open `Day_10_Scene`.
2. Choose **File > Save As**.
3. Save the duplicate as `Day_11_Scene`.
4. Create a Day 11 HumanAgent prefab variant or duplicate the Day 10 agent prefab before modifying it.
5. Confirm that you are editing `Day_11_Scene` before continuing.

### Discussion

Each day preserves the previous completed scene. This gives the class a stable reference and prevents the Day 11 changes from overwriting Day 10.

## Part 4: Modify the scene layout (Optional if not provided)

Arrange the scene so the direct route to the Chest contains a hazard first and a wall later.

Suggested layout:

```text
#########################
#                  C    #
#                       #
#      #######          #
#                       #
#          T            #
#                       #
# H                     #
#########################
```

```text
H = HumanAgent
T = Spike Trap
C = Chest
# = Wall
```

For the first test, move the wall segment out of the direct route and position the Spike Trap between the HumanAgent and Chest.

### Scene checkpoint

- Spike Trap has a collider that the ray can hit.
- Spike Trap has `DetectionSource` set to `Hazard`.
- Walls have colliders and `DetectionSource` set to `Obstacle`.
- Chest remains assigned as the HumanAgent target.
- The ray will not begin inside another collider.

## Part 5: Expand AgentContext

Open `AgentContext.cs` and locate the public Boolean values. Day 11 adds:

```csharp
public bool IsAvoiding;
```

It also adds the response direction:

```csharp
public Vector3 AvoidanceDirection;
```

The complete Day 11 Core file is:

```csharp
using UnityEngine;

public class AgentContext : MonoBehaviour
{
    #region Public Variables

    public bool IsNearObstacle;
    public bool IsNearHazard;
    public bool IsNearTarget;
    public bool IsAvoiding;
    public GameObject CurrentTarget;
    public DetectionType CurrentDetection;
    public Vector3 AvoidanceDirection;

    #endregion

    #region Public Methods

    /// <summary>
    /// Prints the current environmental and avoidance context.
    /// </summary>
    public void PrintCurrentState()
    {
        string targetName =
            CurrentTarget == null
                ? "None"
                : CurrentTarget.name;

        Debug.Log(
            $"Detection: {CurrentDetection} | " +
            $"Is Near Obstacle: {IsNearObstacle} | " +
            $"Is Near Hazard: {IsNearHazard} | " +
            $"Is Near Target: {IsNearTarget} | " +
            $"Is Avoiding: {IsAvoiding} | " +
            $"Avoidance Direction: {AvoidanceDirection} | " +
            $"Current Target: {targetName}",
            this);
    }

    #endregion
}
```

### Discussion

`IsAvoiding` records whether avoidance currently overrides the goal. `AvoidanceDirection` records the movement direction selected by the sensor.

The sensor writes context. The controller reads context.

### Compile checkpoint

Save the file and confirm Unity compiles without errors.

## Part 6: Add AvoidanceSensor

Attach `AvoidanceSensor` to HumanAgent.

The Starter file already contains:

- detection distance
- sensor height
- avoidance visualization distance
- an AgentContext field
- Unity method stubs
- a `ShouldAvoid` helper method

### Step 1: Get AgentContext

Replace `Awake` with:

```csharp
private void Awake()
{
    context =
        GetComponent<AgentContext>();
}
```

### Step 2: Configure the Inspector

Suggested values:

```text
Detection Distance: 2
Sensor Height: 0.5
Avoidance Distance: 1.5
```

Adjust Sensor Height so the red ray intersects the Spike Trap and wall colliders rather than passing below or above them.

### Compile checkpoint

Save and confirm Unity compiles.

## Part 7: Add Debug.DrawRay

At the beginning of `Update`, calculate the sensor origin and direction:

```csharp
Vector3 sensorOrigin =
    transform.position +
    Vector3.up *
    sensorHeight;

Vector3 sensorDirection =
    transform.forward;
```

Draw the ray:

```csharp
Debug.DrawRay(
    sensorOrigin,
    sensorDirection *
    detectionDistance,
    Color.red);
```

### Discussion

The origin is raised so the ray intersects useful colliders. `transform.forward` represents the direction the agent currently faces. The SimpleAgentController will rotate the agent toward its selected movement direction.

`Debug.DrawRay` is a runtime debugging tool. View it in the Scene window while Play Mode is active.

### Testing checkpoint

1. Enter Play Mode.
2. Select HumanAgent.
3. View the Scene window.
4. Confirm a red ray projects forward.
5. Adjust Sensor Height or Detection Distance if the ray misses the Spike Trap.

Exit Play Mode.

## Part 8: Detect hazards first

Add a raycast after `Debug.DrawRay`:

```csharp
bool hasHit =
    Physics.Raycast(
        sensorOrigin,
        sensorDirection,
        out RaycastHit hit,
        detectionDistance);
```

Clear avoidance when nothing is detected:

```csharp
if (!hasHit)
{
    context.IsAvoiding = false;
    context.AvoidanceDirection =
        Vector3.zero;
    return;
}
```

Read the Day 10 classification component:

```csharp
DetectionSource source =
    hit.collider.GetComponent<DetectionSource>();
```

For the first implementation, make `ShouldAvoid` recognize hazards only:

```csharp
private bool ShouldAvoid(
    DetectionSource source)
{
    if (source == null)
    {
        return false;
    }

    return
        source.Type == DetectionType.Hazard;
}
```

Then complete the condition in `Update`:

```csharp
if (!ShouldAvoid(source))
{
    context.IsAvoiding = false;
    context.AvoidanceDirection =
        Vector3.zero;
    return;
}

context.IsAvoiding = true;
context.AvoidanceDirection =
    transform.right;
```

### Discussion

The ray detects any collider, but `ShouldAvoid` determines whether the detected object should change movement. `transform.right` provides a simple rule-based steering direction.

### Compile checkpoint

Save and confirm Unity compiles.

### Sensor testing checkpoint

Temporarily disable SimpleAgentController or set Move Speed to zero.

1. Place the Spike Trap inside the red ray.
2. Enter Play Mode.
3. Inspect AgentContext.
4. Confirm `IsAvoiding` becomes true.
5. Confirm `AvoidanceDirection` points along the HumanAgent's local right direction.
6. Move the Spike Trap outside the ray.
7. Confirm `IsAvoiding` becomes false and AvoidanceDirection returns to zero.

## Part 9: Override movement direction

Open `SimpleAgentController.cs`.

### Step 1: Restore component setup

Replace `Awake` with:

```csharp
private void Awake()
{
    context =
        GetComponent<AgentContext>();

    rigidBody =
        GetComponent<Rigidbody>();

    rigidBody.useGravity = false;
    rigidBody.constraints =
        RigidbodyConstraints.FreezePositionY |
        RigidbodyConstraints.FreezeRotationX |
        RigidbodyConstraints.FreezeRotationZ;
}
```

Y rotation remains available so the agent can face its movement direction.

### Step 2: Calculate the goal direction

At the beginning of `FixedUpdate`:

```csharp
if (context.CurrentTarget == null)
{
    return;
}

Vector3 targetPosition =
    context.CurrentTarget.transform.position;

Vector3 targetDirection =
    targetPosition -
    rigidBody.position;

targetDirection.y = 0f;
```

### Step 3: Respect stop distance only while pursuing the target

```csharp
if (!context.IsAvoiding &&
    targetDirection.magnitude <= stopDistance)
{
    return;
}
```

### Step 4: Select one competing direction

```csharp
Vector3 movementDirection =
    context.IsAvoiding
        ? context.AvoidanceDirection
        : targetDirection;
```

Then flatten and validate it:

```csharp
movementDirection.y = 0f;

if (movementDirection.sqrMagnitude <= 0f)
{
    return;
}

movementDirection.Normalize();
```

### Discussion

The controller now resolves two competing goals:

```text
Normal goal:
Move toward Chest

Temporary higher priority:
Move away from detected hazard
```

Avoidance overrides the goal only while `IsAvoiding` is true.

### Compile checkpoint

Save and confirm Unity compiles.

## Part 10: Rotate and move the agent

The sensor points along `transform.forward`, so the HumanAgent should face its selected movement direction.

Calculate the target rotation:

```csharp
Quaternion targetRotation =
    Quaternion.LookRotation(
        movementDirection,
        Vector3.up);
```

Smoothly rotate:

```csharp
Quaternion nextRotation =
    Quaternion.Slerp(
        rigidBody.rotation,
        targetRotation,
        turnSpeed *
        Time.fixedDeltaTime);
```

Calculate movement:

```csharp
Vector3 nextPosition =
    rigidBody.position +
    movementDirection *
    moveSpeed *
    Time.fixedDeltaTime;
```

Apply both changes:

```csharp
rigidBody.MoveRotation(
    nextRotation);

rigidBody.MovePosition(
    nextPosition);
```

### Hazard avoidance checkpoint

1. Re-enable SimpleAgentController or restore Move Speed.
2. Start the HumanAgent with the Spike Trap on the direct route.
3. Enter Play Mode.
4. Confirm the agent initially faces and moves toward the Chest.
5. Confirm the sensor activates before the agent enters the trap.
6. Confirm the agent turns right while avoiding.
7. Confirm the agent returns toward the Chest after the ray clears the trap.

### Expected limitation

A single forward ray and fixed right turn may cause oscillation or poor choices in some layouts. That limitation is useful. It demonstrates why sensor arrangements and steering rules matter.

## Part 11: Add Gizmos

`Debug.DrawRay` requires Play Mode. Gizmos can show the configured sensor in the Scene window while editing.

Add the red sensor line:

```csharp
private void OnDrawGizmos()
{
    Vector3 sensorOrigin =
        transform.position +
        Vector3.up *
        sensorHeight;

    Gizmos.color = Color.red;
    Gizmos.DrawLine(
        sensorOrigin,
        sensorOrigin +
        transform.forward *
        detectionDistance);
}
```

### Gizmo checkpoint

1. Exit Play Mode.
2. Select HumanAgent.
3. Confirm the red sensor line appears in the Scene window.
4. Change Detection Distance in the Inspector.
5. Confirm the line length updates.

## Part 12: Visualize avoidance direction

Extend `OnDrawGizmos` after the red line:

```csharp
AgentContext agentContext = context;

if (agentContext == null)
{
    agentContext =
        GetComponent<AgentContext>();
}

if (agentContext == null ||
    !agentContext.IsAvoiding)
{
    return;
}

Gizmos.color = Color.green;
Gizmos.DrawLine(
    transform.position,
    transform.position +
    agentContext.AvoidanceDirection.normalized *
    avoidanceDistance);
```

### Discussion

```text
Red = What the agent senses ahead
Green = The temporary response direction
```

The green line is most useful during Play Mode because avoidance context changes at runtime.

### Testing checkpoint

1. Enter Play Mode with the Spike Trap in the ray.
2. Observe the red ray.
3. Confirm the green direction appears while avoiding.
4. Confirm the green direction disappears when avoidance ends.

## Part 13: Extend avoidance to obstacles

Update `ShouldAvoid`:

```csharp
private bool ShouldAvoid(
    DetectionSource source)
{
    if (source == null)
    {
        return false;
    }

    return
        source.Type == DetectionType.Hazard ||
        source.Type == DetectionType.Obstacle;
}
```

### Discussion

The sensor and movement code do not need to change. One classification rule expands the behaviour from hazards to hazards and obstacles.

### Compile checkpoint

Save and confirm Unity compiles.

### Obstacle checkpoint

1. Move the Spike Trap away from the direct route.
2. Place a wall segment between HumanAgent and Chest.
3. Confirm the wall has `DetectionSource` set to `Obstacle`.
4. Enter Play Mode.
5. Confirm the agent senses the wall before collision.
6. Confirm the green avoidance direction appears.
7. Confirm the agent attempts to turn around the wall.

## Part 14: Test competing goals

Use three layouts.

### Test A: Clear route

Expected:

- IsAvoiding remains false.
- Agent moves directly to the Chest.
- No green Gizmo appears.

### Test B: Hazard on the route

Expected:

- Hazard enters the ray.
- IsAvoiding becomes true.
- Right direction overrides the Chest direction.
- Agent resumes the Chest goal after clearing the hazard.

### Test C: Wall on the route

Expected:

- Obstacle enters the ray.
- IsAvoiding becomes true.
- Avoidance overrides the Chest direction.
- The simple implementation may struggle with long walls or corners.

### Discussion checkpoint

Ask:

1. Which direction is the normal goal?
2. Which direction wins during avoidance?
3. What causes the system to return to its normal goal?
4. Why is sensing before impact different from responding after collision?
5. What limitations come from using one ray and one fixed turn direction?

## Part 15: Full scene test

Arrange both a Spike Trap and wall so the agent encounters them during one attempt to reach the Chest.

Complete checklist:

- `Day_11_Scene` is saved.
- HumanAgent has AvoidanceSensor.
- AgentContext includes IsAvoiding and AvoidanceDirection.
- Red runtime ray appears.
- Red edit-time Gizmo appears.
- Green avoidance Gizmo appears while avoiding.
- Hazard activates avoidance.
- Obstacle activates avoidance.
- Avoidance temporarily overrides target pursuit.
- Agent returns to target pursuit when the sensor clears.
- No exceptions appear in the Console.

## Troubleshooting

### The ray points in the wrong direction

- Confirm the visible Human model faces the root GameObject's positive Z direction.
- Confirm SimpleAgentController rotates the Rigidbody.
- Confirm the sensor is attached to the same root object.

### The ray misses the trap or wall

- Adjust Sensor Height.
- Increase Detection Distance.
- Confirm the obstacle collider reaches the sensor height.
- Confirm the trap uses a non-trigger sensing collider if the ray must hit it. A ray can hit triggers depending on project query settings, but a dedicated sensing collider produces a clearer classroom setup.

### The agent never avoids

- Confirm DetectionSource is on the same object as the collider that the ray hits.
- Confirm the source Type is Hazard or Obstacle.
- Confirm AgentContext and AvoidanceSensor are on HumanAgent.

### The agent spins or oscillates

- Reduce Turn Speed.
- Increase Detection Distance.
- Shorten the wall used for the first demonstration.
- Explain that a single sensor and fixed right turn are intentionally limited.

### The agent stops moving near the hazard

- Confirm AvoidanceDirection is not zero.
- Confirm Rigidbody X and Z position are not frozen.
- Confirm Move Speed is greater than zero.

### Duplicate class errors

Do not place Starter and Complete versions in Unity simultaneously.

## Additional practice

### Challenge 1: Left and right sensors

Add angled left and right rays. Choose the clearer side when the forward ray detects an object.

### Challenge 2: Different responses

Turn away more strongly from a Hazard than from an Obstacle.

### Challenge 3: Avoidance strength

Add an `avoidanceStrength` value and combine target and avoidance directions instead of fully overriding the target.

### Challenge 4: Target Gizmo

Draw a blue Gizmo line from HumanAgent to CurrentTarget.

### Challenge 5: Sensor colours

Draw a clear sensor in yellow and a blocked sensor in red.

### Challenge 6: Improved steering

Use the hit normal, angled sensors, or a short memory timer to reduce oscillation.
