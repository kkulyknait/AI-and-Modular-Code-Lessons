# Day 10 Walkthrough: Collision Detection and Context

## Demo purpose

In this demonstration, you will begin with a supplied dungeon scene and build the scripts that let a Human character move, keep the camera centred, detect obstacles, detect trigger volumes, and store environmental context.

Near the end of the demonstration, the Human changes from a player-controlled character into a basic Rigidbody-controlled AI agent that moves toward the Chest.

The central idea is:

```text
Detection produces information.
Information becomes context.
Context can be used by an AI agent.
```

## Part 1: Create the Module 3 project

1. Open Unity Hub.
2. Create a new 3D URP project for Module 3.
3. Give the project a clear name such as `GMPR2010_Module3`.
4. Open the project.

All later Module 3 demonstrations will continue from this project.

## Part 2: Import the supplied starter package

1. Import the supplied Day 10 `.unitypackage`.
2. Include the scene, prefabs, models, materials, and supporting assets.
3. Open the scene named `Day_10_Scene`.
4. Save the project.

The supplied scene should already contain the floor, walls, camera, Human, Chest, and Spike Trap. The code is built during the demo.

### Scene checkpoint

Before adding code, confirm:

- The Human stands on the floor.
- The camera views the room from above at approximately 45 degrees.
- Walls have non-trigger colliders.
- The Spike Trap can receive a trigger collider.
- The Chest can receive a larger trigger collider for target proximity.

### Checkpoint

Expected result:

- No compile errors.
- The Input Actions asset contains a `Player` Action Map.
- The Action Map contains a `Move` Vector2 action.
- Move has WASD and gamepad left-stick bindings.

## Part 5: Configure detection sources

Day 10 avoids relying on Tags and Layers for classification. Those systems will be introduced later in the module. Instead, each relevant scene object receives a `DetectionSource` component.

### Walls

1. Select the wall prefab or wall instances.
2. Add `DetectionSource`.
3. Set Type to `Obstacle`.
4. Confirm the wall collider is not a trigger.

### Spike Trap

1. Select the Spike Trap.
2. Add or select its Box Collider.
3. Enable `Is Trigger`.
4. Add `DetectionSource` to the same GameObject as the trigger collider.
5. Set Type to `Hazard`.

### Chest target zone

1. Select the Chest.
2. Add or select it's Box Collider.
3. Enable `Is Trigger`.
4. Increase the trigger size so the Human can enter the target zone before the AI conversion.
5. Add `DetectionSource` to the same child GameObject.
6. Set Type to `Target`.

The trigger object becomes the stored target. Position it at the Chest so moving toward the trigger also moves toward the Chest.

## Part 6: Configure the Human

Select the Human and add:

- `CharacterController`
- `PlayerInput`
- `AgentContext`
- `HumanController`
- `DetectionAgent`

Configure `PlayerInput`:

```text
Actions: Day10InputActions
Default Map: Player
Behavior: Invoke Unity Events
```

Expand the Player events and connect:

```text
Move
→ HumanController.OnMove
```

The HumanController Starter file contains the component fields and method structure, but the methods are incomplete.

## Part 7: Build the player controller

Open `HumanController.cs`.

### Step 1: Get the CharacterController

Replace `Awake` with:

```csharp
private void Awake()
{
    characterController =
        GetComponent<CharacterController>();
}
```

### Step 2: Read Move input

Replace `OnMove` with:

```csharp
public void OnMove(
    InputAction.CallbackContext context)
{
    moveInput =
        context.ReadValue<Vector2>();
}
```

The input action supplies a two-dimensional value. Its x value controls left and right movement. Its y value controls forward and backward movement on the dungeon floor.

### Step 3: Move the Human

Replace `Update` with:

```csharp
private void Update()
{
    Vector3 movement =
        new Vector3(
            moveInput.x,
            0f,
            moveInput.y);

    characterController.Move(
        movement.normalized *
        moveSpeed *
        Time.deltaTime);
}
```

Normalizing prevents diagonal movement from being faster than movement along one axis.

### Player movement checkpoint

1. Enter Play Mode.
2. Move with WASD.
3. Confirm the Human moves over the floor.
4. Move diagonally and confirm the speed remains consistent.
5. Walk into a wall and confirm the CharacterController prevents movement through it.

At this point, the wall is producing a physical response, but the code is not yet recording what was contacted.

## Part 8: Build the camera follow script

Open `CameraFollow.cs`.

### Step 1: Store the starting offset

Replace `Awake` with:

```csharp
private void Awake()
{
    if (target == null)
    {
        return;
    }

    offset =
        transform.position -
        target.position;
}
```

### Step 2: Follow after movement

Replace `LateUpdate` with:

```csharp
private void LateUpdate()
{
    if (target == null)
    {
        return;
    }

    transform.position =
        target.position +
        offset;
}
```

Add `CameraFollow` to the camera and assign the Human as the target.

`LateUpdate` runs after the Human's regular `Update`, so the camera follows the position established during that frame.

### Camera checkpoint

1. Enter Play Mode.
2. Move in all four directions.
3. Confirm the camera maintains its rotation and height.
4. Confirm the Human remains centred in the view.

## Part 9: Review the context types

Open `DetectionType.cs`:

```csharp
public enum DetectionType
{
    None,
    Obstacle,
    Hazard,
    Target
}
```

Open `AgentContext.cs`.

The Boolean fields use `Is` naming because each one reads as a state question:

```csharp
IsNearObstacle
IsNearHazard
IsNearTarget
```

`CurrentTarget` stores a scene reference. `CurrentDetection` stores a readable category. `PrintCurrentState` writes all current context values to the Console.

This object stores information. It does not decide how the Human or AI should respond.

## Part 10: Build player collision detection

Open `DetectionAgent.cs`.

### Step 1: Get AgentContext

Replace `Awake` with:

```csharp
private void Awake()
{
    context =
        GetComponent<AgentContext>();
}
```

### Step 2: Detect CharacterController contact

A CharacterController reports physical contact through `OnControllerColliderHit`. Replace that method with:

```csharp
private void OnControllerColliderHit(
    ControllerColliderHit hit)
{
    DetectionSource source =
        hit.collider.GetComponent<DetectionSource>();

    if (source == null ||
        source.Type != DetectionType.Obstacle)
    {
        return;
    }

    context.IsNearObstacle = true;
    context.CurrentDetection =
        DetectionType.Obstacle;

    Debug.Log(
        $"Player contacted obstacle: {hit.gameObject.name}",
        this);

    context.PrintCurrentState();
}
```

### Collision checkpoint

1. Enter Play Mode.
2. Walk into several walls.
3. Confirm the Human does not move through them.
4. Confirm obstacle messages appear in the Console.
5. Inspect the Human's AgentContext while touching a wall.

Discuss the difference:

```text
CharacterController collision response
prevents movement through the wall.

DetectionAgent records that the wall was contacted.
```

## Part 11: Build trigger detection

### Step 1: Enter trigger volumes

Replace `OnTriggerEnter` with the Complete version below:

```csharp
private void OnTriggerEnter(
    Collider other)
{
    DetectionSource source =
        other.GetComponent<DetectionSource>();

    if (source == null)
    {
        return;
    }

    if (source.Type == DetectionType.Hazard)
    {
        context.IsNearHazard = true;
        context.CurrentDetection =
            DetectionType.Hazard;

        Debug.Log(
            $"Hazard detected: {other.gameObject.name}",
            this);
    }
    else if (source.Type == DetectionType.Target)
    {
        context.IsNearTarget = true;
        context.CurrentTarget =
            other.gameObject;
        context.CurrentDetection =
            DetectionType.Target;

        Debug.Log(
            $"Target detected: {other.gameObject.name}",
            this);
    }

    context.PrintCurrentState();
}
```

### Step 2: Exit trigger volumes

Replace `OnTriggerExit` with:

```csharp
private void OnTriggerExit(
    Collider other)
{
    DetectionSource source =
        other.GetComponent<DetectionSource>();

    if (source == null)
    {
        return;
    }

    if (source.Type == DetectionType.Hazard)
    {
        context.IsNearHazard = false;

        Debug.Log(
            $"Hazard cleared: {other.gameObject.name}",
            this);
    }
    else if (source.Type == DetectionType.Target)
    {
        context.IsNearTarget = false;

        Debug.Log(
            $"Target proximity cleared: {other.gameObject.name}",
            this);
    }

    RefreshCurrentDetection();
    context.PrintCurrentState();
}
```

### Step 3: Refresh the displayed detection

Add this method:

```csharp
private void RefreshCurrentDetection()
{
    if (context.IsNearTarget)
    {
        context.CurrentDetection =
            DetectionType.Target;
    }
    else if (context.IsNearHazard)
    {
        context.CurrentDetection =
            DetectionType.Hazard;
    }
    else if (context.IsNearObstacle)
    {
        context.CurrentDetection =
            DetectionType.Obstacle;
    }
    else
    {
        context.CurrentDetection =
            DetectionType.None;
    }
}
```

### Trigger checkpoint

1. Enter Play Mode.
2. Walk onto the Spike Trap.
3. Confirm `IsNearHazard` becomes true.
4. Walk off the Spike Trap.
5. Confirm `IsNearHazard` becomes false.
6. Enter the Chest target zone.
7. Confirm `IsNearTarget` becomes true.
8. Confirm `CurrentTarget` stores the target-zone GameObject.
9. Leave the target zone.
10. Confirm `IsNearTarget` becomes false.
11. Confirm `CurrentTarget` remains stored.

Keeping `CurrentTarget` after leaving the proximity zone is intentional for this demonstration. It gives the basic AI a remembered goal without introducing a separate memory system.

## Part 12: Compare controller and Rigidbody collisions

Key comparison:

```text
Player phase:
CharacterController
OnControllerColliderHit

AI phase:
Rigidbody
OnCollisionEnter
OnCollisionExit
```

Both mechanisms can provide obstacle context, but their movement components and physics callbacks differ.

## Part 13: Build the simple AI controller

Open `SimpleAgentController.cs`.

### Step 1: Get and configure components

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
        RigidbodyConstraints.FreezeRotation;
}
```

### Step 2: Move toward the stored target

Replace `FixedUpdate` with:

```csharp
private void FixedUpdate()
{
    if (context.CurrentTarget == null)
    {
        return;
    }

    Vector3 targetPosition =
        context.CurrentTarget.transform.position;

    Vector3 direction =
        targetPosition -
        rigidBody.position;

    direction.y = 0f;

    if (direction.magnitude <= stopDistance)
    {
        return;
    }

    Vector3 nextPosition =
        rigidBody.position +
        direction.normalized *
        moveSpeed *
        Time.fixedDeltaTime;

    rigidBody.MovePosition(
        nextPosition);
}
```

This is intentionally direct movement. It does not avoid walls or traps. Those limitations create the starting point for later demonstrations.

## Part 14: Convert the Human into an AI agent

Before switching controllers, enter the Chest target zone at least once so `CurrentTarget` is stored.

Exit Play Mode and configure the Human:

1. Disable or remove `PlayerInput`.
2. Disable `HumanController`.
3. Disable or remove `CharacterController`.
4. Add `SimpleAgentController`.
5. Allow Unity to add the required Rigidbody.
6. Confirm the Rigidbody has rotation frozen and gravity disabled when Play Mode begins.
7. Keep `AgentContext` and `DetectionAgent` on the Human.

Because Play Mode resets serialized runtime state, assign the Chest target-zone GameObject to `CurrentTarget` in `AgentContext` for the final AI checkpoint. Explain that later demos will replace this temporary setup with active sensing and decision logic.

### AI checkpoint

1. Enter Play Mode.
2. Confirm the Human moves toward the Chest.
3. Confirm the camera continues following the Human.
4. Observe what happens if a wall blocks the direct route.
5. Observe what happens if the direct route crosses the Spike Trap.
6. Confirm Rigidbody obstacle contacts use the collision callbacks.

Expected result:

- The agent moves directly toward its target.
- It can detect hazards but does not yet avoid them.
- It can collide with obstacles but does not yet steer around them.
- It stops near the target according to `stopDistance`.

## Part 15: Final comparison

### Player-controlled Human

```text
Input determines movement.
CharacterController provides physical response.
OnControllerColliderHit records obstacle contact.
Trigger callbacks record hazards and targets.
```

### AI-controlled Human

```text
Stored context determines the goal.
Rigidbody.MovePosition produces movement.
Collision callbacks record obstacle contact.
Trigger callbacks still record hazards and targets.
```

The sensing component remains attached in both cases. What changes is the movement controller and how context is used.

## Complete test checklist

- The project opens without compile errors.
- `Day_10_Scene` opens successfully.
- WASD moves the Human during the player phase.
- The camera follows while preserving its angle and offset.
- Walls physically block the CharacterController.
- Wall contact prints obstacle context.
- Entering and leaving the Spike Trap changes hazard context.
- Entering and leaving the Chest zone changes target proximity context.
- The detected target can be stored in `CurrentTarget`.
- The Human can be converted to a Rigidbody agent.
- The AI moves toward the Chest target.
- The AI stops within `stopDistance`.
- Rigidbody obstacle contact produces collision messages.
- No exceptions appear in the Console.

## Troubleshooting

### Input event does not fire

- Confirm `PlayerInput` uses `Day10InputActions`.
- Confirm Default Map is `Player`.
- Confirm Behavior is `Invoke Unity Events`.
- Confirm the Move event references `HumanController.OnMove`.

### Human does not move

- Confirm `CharacterController` exists and is enabled.
- Confirm `HumanController` is enabled.
- Confirm the Move event sends a Vector2 value.
- Confirm move speed is greater than zero.

### Camera does not follow

- Assign the Human to CameraFollow's target field.
- Confirm CameraFollow is enabled.
- Confirm the camera is not parented to another moving object.

### Trigger callbacks do not fire

- Confirm the trigger collider has `Is Trigger` enabled.
- Confirm `DetectionSource` is on the same GameObject as the collider.
- Confirm the Human has an enabled CharacterController during the player phase or Rigidbody during the AI phase.

### Obstacle is not classified

- Confirm the wall collider's GameObject has `DetectionSource`.
- Confirm its Type is `Obstacle`.

### AI does not move

- Confirm `SimpleAgentController` and Rigidbody are present.
- Confirm `CurrentTarget` is assigned.
- Confirm HumanController and CharacterController are disabled.
- Confirm Rigidbody constraints do not freeze X or Z position.

### Duplicate class errors

Do not copy Starter and Complete scripts into the project at the same time.

## Additional practice

### Challenge 1: Face the movement direction

Rotate the AI so it faces the direction it is moving.

### Challenge 2: Clear the remembered target

Change `OnTriggerExit` so leaving the target zone clears `CurrentTarget`. Describe how that changes the AI conversion.

### Challenge 3: Add a second target

Add another Chest target zone and observe which target is stored most recently.

### Challenge 4: Add a state timer

Print the complete context once per second instead of only when callbacks occur.

### Challenge 5: Camera smoothing

Replace the direct camera position assignment with a smooth follow while preserving the original offset.
