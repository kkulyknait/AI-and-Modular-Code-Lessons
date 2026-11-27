# Day 25 State and Transition Tables

## State Table

| State | Purpose | Behaviour shown by the demo | Outgoing transition checks |
|---|---|---|---|
| Patrol | Normal guard activity | Follow the patrol route and watch for the player | Is Player Visible? |
| Chase | Pursue the detected player | Follow the player using the agent's existing systems | Is Player Visible false? |
| Search | Investigate after losing the player | Search the player's last known area | Is Player Visible? Is Search Complete? |

## Transition Table

Transition order is significant when more than one condition could be true.

| Priority | Current State | Condition | Next State | Logged reason |
|---:|---|---|---|---|
| 1 | Patrol | `IsPlayerVisible == true` | Chase | Player became visible |
| 1 | Chase | `IsPlayerVisible == false` | Search | Player was lost |
| 1 | Search | `IsPlayerVisible == true` | Chase | Player became visible during Search |
| 2 | Search | `IsSearchComplete == true` | Patrol | Search completed without finding the player |

## Test Sequence

| Step | Inspector values | Expected Current State |
|---:|---|---|
| 1 | Player Visible off; Search Complete off | Patrol |
| 2 | Turn Player Visible on | Chase |
| 3 | Turn Player Visible off | Search |
| 4 | Turn Player Visible on | Chase |
| 5 | Turn Player Visible off | Search |
| 6 | Leave Player Visible off; turn Search Complete on | Patrol |

## Flowchart

![Day 25 FSM Flowchart](day25-fsm-flowchart.png)
