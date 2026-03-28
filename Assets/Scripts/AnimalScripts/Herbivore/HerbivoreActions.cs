using System.Collections.Generic;
using UnityEngine;

public class HerbivoreActions : MonoBehaviour
{

    [Header("EatSettings"), SerializeField] float eatspeed = 3f, eatTimer = 0, eatAmount = 5f;
    private bool isWaiting = false;
    [SerializeField] float waitTimer = 1f;
    HexCell currentTarget;
    Queue<HexCell> path = new Queue<HexCell>();


    GrassManager grassManager;
    HerbivoreStats stats;
    Agent agent;
    HerbivoreAi myBrain;
    public HexWorldGenerator world;

    public enum HerbivoreStates
    {
        Wander,
        SeekFood,
        EatFood,
        FallBak,
        Escape
    }

    public HerbivoreStates currentState = HerbivoreStates.Wander;

    private void Start()
    {
        myBrain = GetComponent<HerbivoreAi>();
        agent = GetComponent<Agent>();
        stats = GetComponent<HerbivoreStats>();
        ChooseWanderDestination();
    }

    private void Update()
    {
        if (currentState == HerbivoreStates.Wander)
        {
            Wander();
        }
        else if (currentState == HerbivoreStates.SeekFood)
        {
            SeekFood();
        }
        else if (currentState == HerbivoreStates.EatFood)
        {
            EatFood();
        }
        else if (currentState == HerbivoreStates.Escape)
        {
            Escape();
        }
        // Add your other states here later (EatFood, Escape, etc.)
    }

    private int tigerCount = 0; // track how many tigers are in range

    private Transform nearestThreat = null;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CarnivoreStats>() != null)
        {
            tigerCount++;
            nearestThreat = other.transform; // 
            currentState = HerbivoreStates.Escape;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<CarnivoreStats>() != null)
        {
            tigerCount--;
            if (tigerCount <= 0)
            {
                tigerCount = 0;
                currentState = HerbivoreStates.Wander;
            }
        }
    }

    // ==========================================
    // REUSABLE MOVEMENT LOGIC
    // ==========================================

    /// <summary>
    /// Moves the herbivore along its current path. 
    /// Returns TRUE if it has reached the final destination.
    /// </summary>
    private bool FollowPath()
    {
        // If we have no target, we are already "there"
        if (currentTarget == null) return true;

        Vector3 targetPosition = new Vector3(
            currentTarget.transform.position.x,
            transform.position.y,
            currentTarget.transform.position.z
        );

        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
        float moveStep = stats.moveSpeed * Time.deltaTime;

        // 1. Handle Rotation
        if (distanceToTarget > 0.05f)
        {
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * stats.turnSpeed);
            }
        }

        // 2. Handle Movement & Path Nodes
        if (distanceToTarget <= moveStep)
        {
            transform.position = targetPosition;
            stats.x = currentTarget.x;
            stats.z = currentTarget.z;

            // Are there more steps in the path?
            if (path.Count > 0)
            {
                currentTarget = path.Dequeue();
                return false; // Still walking
            }
            else
            {
                currentTarget = null;
                
                return true; // Reached final destination!

            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveStep);
            return false; // Still walking
        }
    }

    // ==========================================
    // STATE BEHAVIORS
    // ==========================================

    [SerializeField] private float fleePathCooldown = 1f;
    private float fleeTimer = 0f;
    private Agent threatTiger = null;

    private void Escape()
    {
        stats.moveSpeed = stats.runSpeed;
        if (nearestThreat == null) { currentState = HerbivoreStates.Wander; return; }

        fleeTimer -= Time.deltaTime;
        if (fleeTimer <= 0f)
        {
            fleeTimer = fleePathCooldown;

            Vector3 fleeDir = (transform.position - nearestThreat.position).normalized;
            HexCell myCell = world.GetCell(stats.x, stats.z);
            HexCell bestCell = null;
            float bestDot = -Mathf.Infinity;

            foreach (HexCell neighbor in myCell.neighbors)
            {
                if (neighbor == null || neighbor.isWater || neighbor.isTree) continue;
                Vector3 dir = (neighbor.transform.position - transform.position).normalized;
                float dot = Vector3.Dot(dir, fleeDir);
                if (dot > bestDot) { bestDot = dot; bestCell = neighbor; }
            }

            if (bestCell != null)
            {
                HexCell myCell2 = world.GetCell(stats.x, stats.z);
                List<HexCell> newPath = agent.AStar(myCell2, bestCell);
                if (newPath != null && newPath.Count > 0)
                {
                    path = new Queue<HexCell>(newPath);
                    currentTarget = path.Dequeue();
                }
            }
        }

        FollowPath();
    }

    HexCell targetGrassCell = null;
    private void SeekFood()
    {
        stats.moveSpeed = stats.walkSpeed;
        // 1. Find food and calculate a path (only do this once)
        // REMOVED "&& currentTarget == null" so it forces a search!
        if (targetGrassCell == null)
        {
            targetGrassCell = stats.FindClosestCell("GrassHex");

            if (targetGrassCell == null)
            {
                currentState = HerbivoreStates.Wander; // Give up, no grass
                return;
            }

            // Grab the manager
            grassManager = targetGrassCell.GetComponent<GrassManager>();

            // CLEAR the old wander path so we don't accidentally walk there
            path.Clear();

            HexCell start = world.GetCell(stats.x, stats.z);
            List<HexCell> newPath = agent.AStar(start, targetGrassCell);

            if (newPath != null && newPath.Count > 0)
            {
                path = new Queue<HexCell>(newPath);
                currentTarget = path.Dequeue();
            }
            else
            {
                targetGrassCell = null;
                currentState = HerbivoreStates.Wander; // Give up, no path
                return;
            }
        }

        // 2. Walk towards the food
        bool arrived = FollowPath();

        // 3. Check if we reached the food
        if (arrived)
        {
            currentState = HerbivoreStates.EatFood;
            stats.isEating = true;
            targetGrassCell = null; // Reset for next time
        }
    }

    private void EatFood()
    {
        if (grassManager == null || grassManager.grassAmount <= 0)
        {
            stats.isEating = false;
            currentState = HerbivoreStates.SeekFood;
            return;
        }
        eatTimer += Time.deltaTime;
        if (eatTimer > eatspeed)    
        {
            stats.hunger -= eatAmount;
            grassManager.ConsumeGrass(eatAmount);
            eatTimer = 0;
            if (grassManager.grassAmount == 0)
            {
                stats.isEating = false;
                currentState=HerbivoreStates.SeekFood;
            }
        }
    }

    private void Wander()
    {
        stats.moveSpeed=stats.walkSpeed;
        // 1. Handle Waiting
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                isWaiting = false;
                ChooseWanderDestination();
            }
            return;
        }

        // 2. Failsafe if we have no target
        if (currentTarget == null)
        {
            ChooseWanderDestination();
            return;
        }

        // 3. Walk towards random destination
        bool arrived = FollowPath();

        // 4. Check if we finished the wander path
        if (arrived)
        {
            StartWaiting(stats.waitTime);
        }
    }

    // ==========================================
    // UTILITIES
    // ==========================================

    void StartWaiting(float timeToWait)
    {
        isWaiting = true;
        waitTimer = timeToWait;
        currentTarget = null;
    }

    public void ChooseWanderDestination()
    {
        HexCell start = world.GetCell(stats.x, stats.z);

        for (int i = 0; i < 40; i++)
        {
            int rx = stats.homeX + Random.Range(-stats.wanderRadius, stats.wanderRadius + 1);
            int rz = stats.homeZ + Random.Range(-stats.wanderRadius, stats.wanderRadius + 1);

            HexCell goal = world.GetCell(rx, rz);

            if (goal == null || goal.isWater || goal.isTree)
                continue;

            List<HexCell> newPath = agent.AStar(start, goal);

            if (newPath != null && newPath.Count > 0)
            {
                path = new Queue<HexCell>(newPath);
                currentTarget = path.Dequeue();
                return;
            }
        }

        StartWaiting(0.5f); // Failed to find a path 40 times, wait a moment and try again
    }
}