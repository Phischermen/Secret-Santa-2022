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

    // PATHFINDING
    public Transform pathfindingTarget;
    
    private (float distance, bool isWall)[,] _pathfindingGrid;
    private readonly float cellsPerUnit = 0.5f;

    private bool _initialized = false;
    private void InitializePathfindingGrid()
    {
        _initialized = true;
        // Using the arena bounds, create a grid of cells that can be used for pathfinding.
        _pathfindingGrid = new (float,bool)[(int)(arenaBounds.size.x * cellsPerUnit), (int)(arenaBounds.size.y * cellsPerUnit)];
        // Check for collisions in each cell.
        for (int x = 0; x < _pathfindingGrid.GetLength(0); x++)
        {
            for (int y = 0; y < _pathfindingGrid.GetLength(1); y++)
            {
                Vector2 cellPosition = new Vector2(x / cellsPerUnit, y / cellsPerUnit) + (Vector2)arenaBounds.min;
                _pathfindingGrid[x, y] = (float.MaxValue, IsCollision(cellPosition));
            }
        }
    }
    
    private Queue<(int, int)> _openList = new();

    public void UpdatePathGrid(int iterationLimit = 10000)
    {
        if (!_initialized)
        {
            InitializePathfindingGrid();
        }
        else
        {
            // Set the distance of all cells to infinity.
            for (int x = 0; x < _pathfindingGrid.GetLength(0); x++)
            {
                for (int y = 0; y < _pathfindingGrid.GetLength(1); y++)
                {
                    _pathfindingGrid[x, y].distance = float.MaxValue;
                }
            }
        }
        _openList.Clear();
        // Start at the target position.
        Vector2 targetPosition = pathfindingTarget.position;
        int targetX = (int)((targetPosition.x - arenaBounds.min.x) * cellsPerUnit);
        int targetY = (int)((targetPosition.y - arenaBounds.min.y) * cellsPerUnit);
        // Clamp the target position to the grid.
        targetX = Mathf.Clamp(targetX, 0, _pathfindingGrid.GetLength(0) - 1);
        targetY = Mathf.Clamp(targetY, 0, _pathfindingGrid.GetLength(1) - 1);
        _openList.Enqueue((targetX, targetY));
        _pathfindingGrid[targetX, targetY] = (0f, false);
        // Check the cells around the target position.
        var iteration = 0;
        while (_openList.Count > 0)
        {
            iteration += 1;
            if (iteration > iterationLimit)
            {
                return;
            }
            (int x, int y) = _openList.Dequeue();
            float distance = _pathfindingGrid[x, y].distance;
            // Check the cells around the current cell.
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    // Skip the current cell.
                    if (i == 0 && j == 0)
                    {
                        continue;
                    }
                    // Skip cells that are out of bounds.
                    if (x + i < 0 || x + i >= _pathfindingGrid.GetLength(0) || y + j < 0 || y + j >= _pathfindingGrid.GetLength(1))
                    {
                        continue;
                    }
                    // Skip cells that are already closer to the target.
                    if (_pathfindingGrid[x + i, y + j].distance < distance + 1f)
                    {
                        continue;
                    }
                    // Skip cells that are walls.
                    if (_pathfindingGrid[x + i, y + j].isWall)
                    {
                        continue;
                    }
                    // Update the distance to the target.
                    _pathfindingGrid[x + i, y + j] = (distance + 1f, false);
                    // Add the cell to the open list.
                    _openList.Enqueue((x + i, y + j));
                }
            }
        }
    }

    public Vector2 GetMotionTowardsTarget(Vector2 position)
    {
        // Sample the pathfinding grid to get a direction that moves towards the target.
        int x = (int)((position.x - arenaBounds.min.x) * cellsPerUnit);
        int y = (int)((position.y - arenaBounds.min.y) * cellsPerUnit);
        // Bounds check.
        if (x < 0 || x >= _pathfindingGrid.GetLength(0) || y < 0 || y >= _pathfindingGrid.GetLength(1))
        {
            return (Vector2)pathfindingTarget.position - position;
        }
        float distance = _pathfindingGrid[x, y].distance;
        Vector2 motion = Vector2.zero;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                // Skip the current cell.
                if (i == 0 && j == 0)
                {
                    continue;
                }
                // Skip cells that are out of bounds.
                if (x + i < 0 || x + i >= _pathfindingGrid.GetLength(0) || y + j < 0 || y + j >= _pathfindingGrid.GetLength(1))
                {
                    continue;
                }
                // Skip cells that are further from the target.
                if (_pathfindingGrid[x + i, y + j].distance > distance)
                {
                    continue;
                }
                // Update the motion.
                motion = new Vector2(i, j);
                distance = _pathfindingGrid[x + i, y + j].distance;
            }
        }

        return motion;
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
    
    public AnimationCurve politenessWeightCurve;

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
        var idx = (int)(candidates.Count * politenessWeightCurve.Evaluate(Random.value));
        var strategy = candidates[idx];
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
        strategies.OrderBy(strategy => strategy.GetPoliteness()).ToList();
}