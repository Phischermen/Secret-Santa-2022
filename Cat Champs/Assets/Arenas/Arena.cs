using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class Arena : MonoBehaviour
{
    public Bounds arenaBounds;
    public List<Bounds> arenaObstacles;

    // A collision occurs when a point is inside the bounds of an obstacle, or outside the bounds of the arena.
    public bool IsCollision(Vector2 point, float padding = 0f)
    {
        var padArenaBounds = arenaBounds;
        padArenaBounds.Expand(-padding);
        if (!padArenaBounds.Contains(point))
        {
            return true;
        }

        foreach (Bounds obstacle in arenaObstacles)
        {
            var padObstacle = obstacle;
            padObstacle.Expand(padding);
            if (padObstacle.Contains(point))
            {
                return true;
            }
        }

        return false;
    }

    public bool IsCollisionExt(Vector2 start, Vector2 dest, out Vector2 intersectionPoint, out Vector2 resolvingVector)
    {
        Vector2 v1 = Vector2.zero;
        Vector2 v2 = Vector2.zero;
        if (CheckSidesOfBounds(arenaBounds))
        {
            intersectionPoint = v1;
            resolvingVector = v2;
            return true;
        }
        foreach (var arenaObstacle in arenaObstacles)
        {
            if (CheckSidesOfBounds(arenaObstacle))
            {
                intersectionPoint = v1;
                resolvingVector = v2;
                return true;
            }
        }
        intersectionPoint = v1;
        resolvingVector = v2;
        return false;
        bool CheckSidesOfBounds(Bounds bounds)
        {
            // Check top
            if (LineIntersection(start, dest, new Vector2(bounds.min.x, bounds.max.y),
                    new Vector2(bounds.max.x, bounds.max.y), ref v1))
            {
                v2 = new Vector2(1, 0);
                return true;
            }
            // Check bottom
            if (LineIntersection(start, dest, new Vector2(bounds.min.x, bounds.min.y),
                    new Vector2(bounds.max.x, bounds.min.y), ref v1))
            {
                v2 = new Vector2(1, 0);
                return true;
            }
            // Check left
            if (LineIntersection(start, dest, new Vector2(bounds.min.x, bounds.min.y),
                    new Vector2(bounds.min.x, bounds.max.y), ref v1))
            {
                v2 = new Vector2(0, 1);
                return true;
            }
            // Check right
            if (LineIntersection(start, dest, new Vector2(bounds.max.x, bounds.min.y),
                    new Vector2(bounds.max.x, bounds.max.y), ref v1))
            {
                v2 = new Vector2(0, 1);
                return true;
            }

            return false;
        }
    }

    private bool LineIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, ref Vector2 intersection)
    {
        float Ax, Bx, Cx, Ay, By, Cy, d, e, f, num /*,offset*/;

        float x1lo, x1hi, y1lo, y1hi;


        Ax = p2.x - p1.x;

        Bx = p3.x - p4.x;


        // X bound box test/

        if (Ax < 0)
        {
            x1lo = p2.x;
            x1hi = p1.x;
        }
        else
        {
            x1hi = p2.x;
            x1lo = p1.x;
        }


        if (Bx > 0)
        {
            if (x1hi < p4.x || p3.x < x1lo) return false;
        }
        else
        {
            if (x1hi < p3.x || p4.x < x1lo) return false;
        }


        Ay = p2.y - p1.y;

        By = p3.y - p4.y;


        // Y bound box test//

        if (Ay < 0)
        {
            y1lo = p2.y;
            y1hi = p1.y;
        }
        else
        {
            y1hi = p2.y;
            y1lo = p1.y;
        }


        if (By > 0)
        {
            if (y1hi < p4.y || p3.y < y1lo) return false;
        }
        else
        {
            if (y1hi < p3.y || p4.y < y1lo) return false;
        }


        Cx = p1.x - p3.x;

        Cy = p1.y - p3.y;

        d = By * Cx - Bx * Cy; // alpha numerator//

        f = Ay * Bx - Ax * By; // both denominator//


        // alpha tests//

        if (f > 0)
        {
            if (d < 0 || d > f) return false;
        }
        else
        {
            if (d > 0 || d < f) return false;
        }

        e = Ax * Cy - Ay * Cx; // beta numerator//

        // beta tests //
        if (f > 0)
        {
            if (e < 0 || e > f) return false;
        }
        else
        {
            if (e > 0 || e < f) return false;
        }

        // check if they are parallel
        if (f == 0) return false;

        // compute intersection coordinates //
        num = d * Ax; // numerator //

        intersection.x = p1.x + num / f;
        num = d * Ay;
        intersection.y = p1.y + num / f;
        return true;
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
                _pathfindingGrid[x, y] = (float.MaxValue, IsCollision(cellPosition, 2.5f));
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
                    var valueTuple = (x + i, y + j);
                    if (!_openList.Contains(valueTuple))
                    {
                        _openList.Enqueue(valueTuple);
                    }
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
    
    public List<SpawnStrategy> lateGameStrategies;

    // These are the strategies that are currently active.
    public List<SpawnStrategy> activeStrategies;

    [HideInInspector] public float timeSinceArenaStart = 0f;
    [HideInInspector] public float timeToReevaluateStrategy = 0f;
    [HideInInspector] public float timeToDoASpecialEvent;
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
            timeToReevaluateStrategy = timeSinceArenaStart + reevaluationFrequency;
        }
    }

    private List<(float time, int damage)> _damageHistory = new();
    private void StartArena()
    {
        firstTimeMode = PlayerPrefs.GetInt("firstTimeMode", 1) == 1;
        GameplayState.GetPlayer().health.OnDamageTaken += (_, damage) =>
        {
            _damageHistory.Add((timeSinceArenaStart, damage));
        };
        if (firstTimeMode)
        {
            // Do not spawn anything for a little while.
            CommitToRest(5f);
            firstTimeMode = false;
            PlayerPrefs.SetInt("firstTimeMode", 0);
            DOTween.Sequence(this).AppendCallback(() =>
                TextPopup.Create("Welcome to the arena!", Color.white, GameplayState.GetPlayer().transform.position))
                .AppendInterval(1f)
                .AppendCallback(() =>
                    TextPopup.Create("You can move around with WASD and attack with the mouse.", Color.white,
                        GameplayState.GetPlayer().transform.position))
                .AppendInterval(1f)
                .AppendCallback(() =>
                    TextPopup.Create("Survive as long as you can!", Color.white,
                        GameplayState.GetPlayer().transform.position));
        }
        else
        {
            // Pick an early game strategy.
            PickEarlyGameStrategy();
        }
    }

    private void ReevaluateStrategy()
    {
        var recentTotalDamage = 0;
        for (int i = 0; i < _damageHistory.Count; i++)
        {
            // If damage was taken more than 10 seconds ago, stop counting it.
            var valueTuple = _damageHistory[i];
            if (timeSinceArenaStart - valueTuple.time > 10f)
            {
                _damageHistory.RemoveRange(i, _damageHistory.Count - i);
                break;
            }
            recentTotalDamage += valueTuple.damage;
        }
        if (committedTime > timeToCommit)
        {
            // We have exceeded the recommended amount of commitment time for the chosen strategies.
            // Raise intensity of active strategies.
            foreach (var activeStrategy in activeStrategies)
            {
                if (recentTotalDamage < 10f)
                {
                    activeStrategy.intensity += 0.1f;
                }
            }
            PickNewStrategy();
        }
    }

    private void PickNewStrategy()
    {
        if (timeSinceArenaStart < 60f)
        {
            PickEarlyGameStrategy();
        }
        else
        {
            if (timeSinceArenaStart > timeToDoASpecialEvent)
            {
                PickASpecialEvent();
                timeToDoASpecialEvent = timeSinceArenaStart + 60f;
            }
            else
            {
                PickLateGameStrategy();
            }
        }
    }

    private void PickEarlyGameStrategy()
    {
        var candidates = GetLeastPoliteStrategies(earlyGameStrategies);
        var idx = (int)(candidates.Count * politenessWeightCurve.Evaluate(Random.value));
        var strategy = candidates[idx];
        CommitToStrategy(strategy);
    }

    private void PickLateGameStrategy()
    {
        var candidates = GetLeastPoliteStrategies(lateGameStrategies);
        var idx = (int)(candidates.Count * politenessWeightCurve.Evaluate(Random.value));
        var strategy = candidates[idx];
        CommitToStrategy(strategy);
    }

    private void PickASpecialEvent()
    {
        // In the interest of time, only one special event exists and it is to pick two early game strategies.
        var poolToPickFrom = timeSinceArenaStart > 120f ? lateGameStrategies : earlyGameStrategies;
        var candidates = GetLeastPoliteStrategies(poolToPickFrom);
        var idx1 = (int)(candidates.Count * politenessWeightCurve.Evaluate(Random.value));
        var strategy1 = candidates[idx1];
        candidates.RemoveAt(idx1);
        var idx2 = (int)(candidates.Count * politenessWeightCurve.Evaluate(Random.value));
        var strategy2 = candidates[idx2];
        CommitToStrategies(strategy1, strategy2);
    }

    private void CommitToStrategy(SpawnStrategy strategy)
    {
        activeStrategies.Clear();
        activeStrategies.Add(strategy);
        committedTime = 0;
        timeToCommit = strategy.GetCommitmentTime();
    }
    
    private void CommitToStrategies(params SpawnStrategy[] strategies)
    {
        activeStrategies.Clear();
        activeStrategies.AddRange(strategies);
        committedTime = 0;
        timeToCommit = strategies.Min(s => s.GetCommitmentTime());
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