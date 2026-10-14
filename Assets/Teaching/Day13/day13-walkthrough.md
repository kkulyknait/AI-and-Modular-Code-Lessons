# Day 13 Walkthrough: Perception Systems

## Demo purpose

Day 13 adds perception without adding memory, response, or priority. Agents identify what is currently nearby and visible, store every visible object in a collection, and expose tag-based context for later lessons.

```text
Layers -> broad physics filtering
Tags -> important object identity
OverlapSphere -> range
Field of view -> facing
Raycast -> line of sight
SphereCast -> volumetric avoidance
```

## Package structure

```text
day13-perception-systems-revised/
├── day13-walkthrough.md
├── instructor-notes.md
└── Day13/
    ├── Core/
    │   └── AgentContext.cs
    ├── Starter/
    │   ├── AvoidanceSensor.cs
    │   └── PerceptionSensor.cs
    └── Teaching/
        └── Complete/
            ├── Complete_Day13_AgentContext.cs
            ├── Complete_Day13_AvoidanceSensor.cs
            └── Complete_Day13_PerceptionSensor.cs
```

`PerceivedObjectType` and `PerceivableObject` are not used. Layers filter physics queries and tags classify important objects.

## Part 1: Verify Day 12

1. Open the existing Module 3 project.
2. Open `Day_12_Scene`.
3. Confirm chase, flee, and avoidance still work.
4. Confirm each agent root retains a collider and Rigidbody after removing any player controller.
5. Exit Play Mode.

## Part 2: Import the package

1. Import Day 13.
2. Replace AgentContext with the Day 13 Core file.
3. Replace AvoidanceSensor with the Starter version.
4. Add PerceptionSensor.
5. Keep the prefixed complete files in Teaching/Complete.
6. Compile.

## Part 3: Create the scene

1. Save `Day_12_Scene` as `Day_13_Scene`.
2. Create Day 13 prefab variants before modifying layers, tags, or components.

## Part 4: Optional level setup

This section is optional because the starter package may provide a different level.

The scene should support tests with:

- HumanAgent
- OrcAgent
- Chest
- Key
- Powerup
- Spike Trap
- Pit
- walls that can block line of sight

## Part 5: Create and assign layers

Create:

```text
Agent
Pickup
Target
Hazard
Obstacle
Ground
```

Assign:

```text
HumanAgent and OrcAgent -> Agent
Key and Powerup -> Pickup
Chest -> Target
Spike Trap and Pit -> Hazard
Walls -> Obstacle
Floor -> Ground
```

If a collider is on a child, assign the layer to that child as well.

Do not delete `DetectionSource.cs` or `DetectionType.cs` yet. Older scenes and DetectionAgent may still reference them. In Day 13, stop relying on DetectionSource for new sensing and avoidance logic. Remove DetectionSource components from Day 13 obstacles and hazards only after the layer-based replacement is confirmed.

## Part 6: Create and assign tags

Create:

```text
Player
AIAgent
Key
Powerup
Chest
Hazard
```

Assign tags to the logical root objects:

```text
HumanAgent -> Player
OrcAgent -> AIAgent
Key -> Key
Powerup -> Powerup
Chest -> Chest
Spike Trap and Pit -> Hazard
```

Walls need no tag. Their Obstacle layer is enough for line-of-sight obstruction and avoidance.

## Part 7: Expand AgentContext

AgentContext stores the current perception collection and convenient tag-based references.

```csharp
public List<Transform> PerceivedObjects = new List<Transform>();
```

`ClearPerception` resets the current scan. `AddPerceivedObject` stores unique transforms and uses CompareTag to update Agent, Pickup, Target, and Hazard fields.

No memory is retained when an object is no longer visible.

## Part 8: Attach PerceptionSensor

Attach PerceptionSensor to both agents.

Suggested Inspector values:

```text
Perception Radius: 6
Field Of View Angle: 90
Eye Height: 0.5
Perception Mask: Agent, Pickup, Target, Hazard
Obstruction Mask: Obstacle
```

Implement Awake:

```csharp
private void Awake()
{
    context = GetComponent<AgentContext>();
}
```

## Part 9: Find nearby colliders

In Update:

```csharp
context.ClearPerception();
Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, perceptionRadius, perceptionMask, QueryTriggerInteraction.Collide);
```

OverlapSphere proves only that a matching collider is in range.

## Part 10: Resolve child colliders to tagged objects

Physics queries may return a child collider. Add `GetTaggedTransform` so the sensor searches the collider and its parents for a supported tag.

```csharp
private Transform GetTaggedTransform(Transform candidate)
{
    Transform current = candidate;

    while (current != null)
    {
        if (current.CompareTag("Player") || current.CompareTag("AIAgent") || current.CompareTag("Key") || current.CompareTag("Powerup") || current.CompareTag("Chest") || current.CompareTag("Hazard"))
        {
            return current;
        }

        current = current.parent;
    }

    return null;
}
```

In the loop:

```csharp
Transform perceivedTransform = GetTaggedTransform(nearbyCollider.transform);

if (perceivedTransform == null || perceivedTransform.root == transform.root)
{
    continue;
}
```

## Part 11: Filter by field of view

```csharp
private bool IsInsideFieldOfView(Transform candidate)
{
    Vector3 directionToCandidate = candidate.position - transform.position;
    directionToCandidate.y = 0f;

    if (directionToCandidate.sqrMagnitude <= 0f)
    {
        return true;
    }

    directionToCandidate.Normalize();
    float minimumDot = Mathf.Cos(fieldOfViewAngle * 0.5f * Mathf.Deg2Rad);
    float forwardDot = Vector3.Dot(transform.forward, directionToCandidate);
    return forwardDot >= minimumDot;
}
```

## Part 12: Confirm line of sight

```csharp
private bool HasLineOfSight(Transform candidate)
{
    Vector3 origin = transform.position + Vector3.up * eyeHeight;
    Vector3 destination = candidate.position + Vector3.up * eyeHeight;
    Vector3 direction = destination - origin;
    float distance = direction.magnitude;

    if (distance <= 0f)
    {
        return true;
    }

    direction.Normalize();
    Debug.DrawRay(origin, direction * distance, Color.cyan);

    if (!Physics.Raycast(origin, direction, out RaycastHit hit, distance, perceptionMask | obstructionMask, QueryTriggerInteraction.Collide))
    {
        return false;
    }

    return hit.transform.root == candidate.root;
}
```

## Part 13: Complete the perception loop

```csharp
foreach (Collider nearbyCollider in nearbyColliders)
{
    Transform perceivedTransform = GetTaggedTransform(nearbyCollider.transform);

    if (perceivedTransform == null || perceivedTransform.root == transform.root)
    {
        continue;
    }

    if (!IsInsideFieldOfView(perceivedTransform) || !HasLineOfSight(perceivedTransform))
    {
        continue;
    }

    context.AddPerceivedObject(perceivedTransform);
}
```

Test multiple visible agents, pickups, targets, and hazards. Confirm every unique object appears once in PerceivedObjects.

## Part 14: Visualize perception

```csharp
private void OnDrawGizmosSelected()
{
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(transform.position, perceptionRadius);
    Vector3 leftBoundary = Quaternion.Euler(0f, -fieldOfViewAngle * 0.5f, 0f) * transform.forward;
    Vector3 rightBoundary = Quaternion.Euler(0f, fieldOfViewAngle * 0.5f, 0f) * transform.forward;
    Gizmos.color = Color.blue;
    Gizmos.DrawLine(transform.position, transform.position + leftBoundary * perceptionRadius);
    Gizmos.DrawLine(transform.position, transform.position + rightBoundary * perceptionRadius);
}
```

```text
Yellow sphere = range
Blue lines = field of view
Cyan runtime ray = line of sight
```

## Part 15: Upgrade AvoidanceSensor from Raycast to SphereCast

### 15.1 Review the Day 11 sensor

The previous sensor used a thin forward Raycast. It could miss an obstacle edge even when the agent collider was too wide to pass.

### 15.2 Add the new serialized fields

Open AvoidanceSensor and add these fields under Inspector Variables:

```csharp
[SerializeField]
private float sensorRadius = 0.4f;

[SerializeField]
private LayerMask avoidanceMask;
```

Keep the existing fields:

```csharp
[SerializeField]
private float detectionDistance = 2f;

[SerializeField]
private float sensorHeight = 0.5f;
```

`sensorRadius` controls the width of the swept sensing volume. Start near the radius of the agent collider, then tune it for the level scale.

`avoidanceMask` determines which layers can activate avoidance.

### 15.3 Configure avoidanceMask in the Inspector

Select each Day 13 agent. In AvoidanceSensor, open the Avoidance Mask dropdown and enable only:

```text
Obstacle
Hazard
```

Disable:

```text
Agent
Pickup
Target
Ground
```

The agent should avoid walls, Spike Traps, and Pits. It should not steer away merely because another agent, Key, Powerup, Chest, or floor is detected.

If the mask displays `Nothing`, the SphereCast cannot hit any layers. If it displays `Everything`, unrelated objects may trigger avoidance.

### 15.4 Replace component classification

Remove the old code that reads `DetectionSource` from the hit collider. The layer mask now filters acceptable hits before the result is returned.

### 15.5 Calculate the SphereCast origin

```csharp
Vector3 origin = transform.position + Vector3.up * sensorHeight;
```

Raise the origin so the swept sphere intersects the useful part of wall and hazard colliders.

### 15.6 Perform the SphereCast

```csharp
bool hasHit = Physics.SphereCast(origin, sensorRadius, transform.forward, out RaycastHit hit, detectionDistance, avoidanceMask, QueryTriggerInteraction.Collide);
```

The arguments provide:

```text
origin -> starting centre
sensorRadius -> sphere width
transform.forward -> cast direction
hit -> first matching result
detectionDistance -> cast length
avoidanceMask -> accepted layers
QueryTriggerInteraction.Collide -> include trigger hazard volumes
```

### 15.7 Store avoidance context

```csharp
context.IsAvoiding = hasHit;
context.AvoidanceDirection = hasHit ? transform.right : Vector3.zero;
```

### 15.8 Visualize the swept area

```csharp
private void OnDrawGizmosSelected()
{
    Vector3 origin = transform.position + Vector3.up * sensorHeight;
    Vector3 end = origin + transform.forward * detectionDistance;
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(origin, sensorRadius);
    Gizmos.DrawWireSphere(end, sensorRadius);
    Gizmos.DrawLine(origin, end);
}
```

### 15.9 Test and tune

1. Test a direct wall approach.
2. Test a wall corner that the old centre ray could miss.
3. Test the Spike Trap.
4. Test the Pit hazard collider.
5. Confirm the Key, Powerup, Chest, floor, and other agent do not activate avoidance.
6. Reduce sensorRadius if avoidance activates too early.
7. Increase sensorRadius if the agent collider still clips corners.
8. Adjust sensorHeight if the sweep passes above or below a collider.

## Part 16: Compare cast and overlap tools

```text
Raycast = a line test
SphereCast = a sphere swept through space
CapsuleCast = a capsule swept through space
OverlapSphere = colliders currently inside a sphere
OverlapCapsule = colliders currently inside a capsule
```

The completed system uses OverlapSphere for range, Raycast for line of sight, and SphereCast for avoidance.

## Part 17: Compatibility check after removing object classification components

Before removing DetectionSource components from the Day 13 scene:

1. Search the project for `DetectionSource`.
2. Confirm the Day 13 AvoidanceSensor no longer references it.
3. Confirm the Day 13 PerceptionSensor uses layers and tags only.
4. Review DetectionAgent. If it still reads DetectionSource, either keep the required components for that legacy callback demonstration or disable/remove DetectionAgent from the Day 13 prefab.
5. Keep `DetectionSource.cs` and `DetectionType.cs` in the project so older scenes continue compiling.
6. Test chase, flee, perception, and avoidance after component removal.

## Part 18: Full scene test

- layers and tags are assigned to collider-bearing objects
- PerceivedObjects contains all visible tagged objects
- walls block line of sight
- field of view rejects objects behind the agent
- SphereCast detects only Obstacle and Hazard layers
- Key and Powerup are pickups
- Chest is a target
- Spike Trap and Pit are hazards
- no memory, response, or priority has been added
- no Console errors appear

## Troubleshooting

### Object is in range but not stored

Check layer, tag, field of view, obstruction, and whether the tagged root can be found from the collider parent chain.

### SphereCast detects the floor

Remove Ground from avoidanceMask and raise sensorHeight if the sphere overlaps the floor at its start position.

### SphereCast immediately detects the agent

Ensure Agent is not in avoidanceMask and ensure the sphere does not begin inside an Obstacle or Hazard collider.

### Day 10 callbacks stop classifying objects

DetectionAgent may still depend on DetectionSource. Do not remove those components from historical scenes, or remove the legacy component from the Day 13 prefab once the new systems replace its purpose.

## Additional practice

1. Replace OverlapSphere with OverlapCapsule.
2. Replace SphereCast with CapsuleCast.
3. Draw lines to every transform in PerceivedObjects.
4. Use CompareTag to distinguish Key and Powerup.
5. Add multiple pickups and hazards.
6. Add a configurable list of supported tags.
