using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpamStrategy : SpawnStrategy
{
    public float spawnFrequency;
    public List<GameObject> actorsToSpawn;
    [HideInInspector] public float lastTimeSpawned;
    public int commitmentTime;

    private int spawnedActors;
    private int killedActors;
    public override void DoStrategy()
    {
        if (Time.time - lastTimeSpawned > spawnFrequency)
        {
            lastTimeSpawned = Time.time;
            // Spawn one of the actors randomly.
            // Get a random index.
            int index = Random.Range(0, actorsToSpawn.Count);
            // Get the actor at that index. 
            GameObject gobj = actorsToSpawn[index];
            // Get arena bounds.
            Bounds arenaBounds = GameplayState.GetArena().arenaBounds;
            // Pick a random position inside the arena bounds.
            Vector2 point;
            do
            {
                point = new Vector2(Random.Range(arenaBounds.min.x, arenaBounds.max.x),
                    Random.Range(arenaBounds.min.y, arenaBounds.max.y));
            } while (GameplayState.GetArena().IsCollision(point));
            var actor = Spawn(gobj, point);
            actor.health.Defense = 1f + intensity;
            spawnedActors++;
            actor.health.HealthDepleted += _ => killedActors++;
        }
    }

    public override int GetCommitmentTime()
    {
        return commitmentTime;
    }

    protected override int GetPolitenessInternal()
    {
        var timeSinceLastSpawn = Time.time - lastTimeSpawned;
        return spawnedActors + killedActors - (int)timeSinceLastSpawn;
    }
}
