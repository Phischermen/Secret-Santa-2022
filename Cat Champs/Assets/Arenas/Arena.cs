using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Arena : MonoBehaviour
{
    public Bounds arenaBounds;
    public List<Bounds> arenaObstacles;

    // A collision occurs when a point is inside the bounds of an obstacle, or outside the bounds of the arena.
    public bool IsCollision(Vector2 point)
    {
        if (!arenaBounds.Contains(point))
        {
            return true;
        }

        foreach (Bounds obstacle in arenaObstacles)
        {
            if (obstacle.Contains(point))
            {
                return true;
            }
        }

        return false;
    }

    // SPAWNING
    public bool firstTimeMode;

    enum Event
    {
        PickMultipleStrategies,
        IntroduceNewMiniBoss,
        PickToughStrategy,
        MiniBossInMiddleOfStrategy
    }

    // These strategies don't spawn enemies as often.
    public List<SpawnStrategy> restfulSpawnStrategies;

    // These strategies spawn basic enemies at the beginning of the game. 
    public List<SpawnStrategy> earlyGameStrategies;

    // These are the strategies that are currently active.
    public List<SpawnStrategy> activeStrategies;

    [HideInInspector] public float timeSinceArenaStart = 0f;
    [HideInInspector] public float timeToReevaluateStrategy = 0f;
    [HideInInspector] public float committedTime; // Amount of time committed to current set of strategies.
    [HideInInspector] public float timeToCommit;
    public float reevaluationFrequency = 1f;

    public void ControlSpawning()
    {
        foreach (var activeStrategy in activeStrategies)
        {
            activeStrategy.DoStrategy();
        }

        // Check for "start".
        if (timeSinceArenaStart == 0f)
        {
            StartArena();
        }

        // Progress time.
        timeSinceArenaStart += Time.deltaTime;
        committedTime += Time.deltaTime;
        // Check if time to reevaluate strategy.
        if (timeSinceArenaStart >= timeToReevaluateStrategy)
        {
            ReevaluateStrategy();
        }
    }

    private void StartArena()
    {
        if (firstTimeMode)
        {
            // Do not spawn anything for a little while.
            CommitToRest(5f);
        }
        else
        {
            // Pick an early game strategy.
            PickEarlyGameStrategy();
        }
    }

    private void ReevaluateStrategy()
    {
        if (committedTime > timeToCommit)
        {
            // We have exceeded the recommended amount of commitment time for the chosen strategies.
            
            PickNewStrategy();
        }
        // todo Evaluate player's performance and possibly change strategy
    }

    private void PickNewStrategy()
    {
        if (timeSinceArenaStart < 60f)
        {
            PickEarlyGameStrategy();
        }
        else
        {
            //todo PlayASpecialEvent();
            PickEarlyGameStrategy();
        }
    }

    private void PickEarlyGameStrategy()
    {
        var candidates = GetLeastPoliteStrategies(earlyGameStrategies);
        var strategy = candidates[0];
        CommitToStrategy(strategy);
    }

    private void CommitToStrategy(SpawnStrategy strategy)
    {
        activeStrategies.Clear();
        activeStrategies.Add(strategy);
        committedTime = 0;
        timeToCommit += strategy.GetCommitmentTime();
    }

    private void CommitToRest(float time)
    {
        activeStrategies.Clear();
        committedTime = 0;
        timeToCommit = time;
    }

    private List<SpawnStrategy> GetLeastPoliteStrategies(ICollection<SpawnStrategy> strategies) =>
        strategies.OrderByDescending(strategy => strategy.GetPoliteness()).ToList();
}