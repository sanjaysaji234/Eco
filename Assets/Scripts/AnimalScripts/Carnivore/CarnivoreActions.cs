using System.Collections.Generic;
using UnityEngine;

public class CarnivoreActions : MonoBehaviour
{

    [Header("EatSettings"), SerializeField] float eatspeed = 3f, eatTimer = 0;
    [SerializeField] private float failedFindFoodCoolDown=0f,coolDownTimer=3f;
    private bool isWaiting = false;
    [SerializeField] float waitTimer = 1f;
    HexCell currentTarget;
    Queue<HexCell> path = new Queue<HexCell>();

    HerbivoreStats deerStats;
    CarnivoreStats stats;
    Agent agent;
    CarnivoreAi myBrain;
    public HexWorldGenerator world;

    public enum CarnivoreStates
    {
        Wander,
        SeekFood,
        Hunt,
        EatFood,
        FallBak,
        Escape
    }

    public CarnivoreStates currentState = CarnivoreStates.Wander;

    private void Start()
    {
        myBrain = GetComponent<CarnivoreAi>();
        agent = GetComponent<Agent>();
        stats = GetComponent<CarnivoreStats>();
        ChooseWanderDestination();
    }

    private void Update()
    {
        if (failedFindFoodCoolDown > 0)
        {
            failedFindFoodCoolDown-= Time.deltaTime;
        }
        if (currentState == CarnivoreStates.Wander)
        {
            Wander();
        }
        else if (currentState == CarnivoreStates.SeekFood)
        {
            SeekFood();
        }
        else if (currentState == CarnivoreStates.EatFood)
        {
            EatFood();
        }
        else if (currentState == CarnivoreStates.Hunt)
        {
            Hunt();
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

    Agent targetDeer = null;
    private void SeekFood()
    {
        stats.moveSpeed = stats.walkspeed;
        // 1. Find food and calculate a path (only do this once)
        // REMOVED "&& currentTarget == null" so it forces a search!

        if (failedFindFoodCoolDown > 0)
        {
            currentState = CarnivoreStates.Wander;
            return;
        }
        if (targetDeer == null || targetDeer.gameObject == null)
        {
            targetDeer = stats.FindClosestDeer();
            failedFindFoodCoolDown = coolDownTimer;
            if (targetDeer == null)
            {
                currentState = CarnivoreStates.Wander; // Give up, no grass
                return;
            }


            // CLEAR the old wander path so we don't accidentally walk there
            path.Clear();

            HexCell myCell = world.GetCell(stats.x, stats.z);

            deerStats=targetDeer.GetComponent<HerbivoreStats>();
            HexCell targetDeerHomeCell = world.GetCell(deerStats.homeX, deerStats.homeZ);

            List<HexCell> newPath = agent.AStar(myCell, targetDeerHomeCell);

            if (newPath != null && newPath.Count > 0)
            {
                path = new Queue<HexCell>(newPath);
                currentTarget = path.Dequeue();
            }
            else
            {
                targetDeerHomeCell = null;
                currentState = CarnivoreStates.Wander;
                return;
            }
        }
        if (targetDeer == null || targetDeer.gameObject == null)
        {
            targetDeer = null;
            currentState = CarnivoreStates.SeekFood; // restart with new deer
            return;
        }

        float dist = Vector3.Distance(transform.position, targetDeer.transform.position);
        if (dist <= stats.HuntRadius)
        {
            path.Clear();
            currentTarget = null;
            huntPathCalculated = false; // fresh path calc when entering Hunt
            currentState = CarnivoreStates.Hunt;
            return;
        }
        FollowPath();
    }

    private bool huntPathCalculated = false;

    private void Hunt()
    {
        stats.moveSpeed = stats.runSpeed;
        if (targetDeer == null || targetDeer.gameObject == null)
        {
            targetDeer = null;
            currentState = CarnivoreStates.Wander;
            return;
        }

        float dist = Vector3.Distance(transform.position, targetDeer.transform.position);

        // Kill range
        if (dist < 2f)
        {
            huntPathCalculated = false;
            currentState = CarnivoreStates.EatFood;
            stats.isEating = true;
            return;
        }

        // Calculate path once
        if (!huntPathCalculated)
        {
            huntPathCalculated = true;
            HexCell myCell = world.GetCell(stats.x, stats.z);
            HexCell deerCell = world.GetCell(deerStats.x, deerStats.z);
            List<HexCell> newPath = agent.AStar(myCell, deerCell);
            if (newPath != null && newPath.Count > 0)
            {
                path = new Queue<HexCell>(newPath);
                currentTarget = path.Dequeue();
            }
        }

        bool arrived = FollowPath();

        // Arrived but deer not in kill range — recalculate once to new position
        if (arrived && dist >= 2f)
        {
            huntPathCalculated = false;
        }
    


}
    private void EatFood()
    {
        if (deerStats != null)
        {
            deerStats.life = 0;
        }
       
        stats.isEating = true;
        eatTimer += Time.deltaTime;
        if (eatTimer > eatspeed)
        {
            eatTimer = 0;
            stats.hunger = 0;
            stats.isEating = false;
            
            HexCell newHome=stats.FindClosestCell();
            if (newHome!=null)
            {
                stats.homeX = newHome.x;
                stats.homeZ = newHome.z;
            }
            currentState = CarnivoreStates.Wander;
        }
    }

    private void Wander()
    {
        stats.moveSpeed=stats.walkspeed;
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