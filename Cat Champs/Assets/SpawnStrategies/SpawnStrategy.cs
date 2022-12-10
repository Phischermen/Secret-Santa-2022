using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpawnStrategy : MonoBehaviour
{
    /* Spawn strategies determine what enemies to spawn and when to spawn them. */
    public abstract void DoStrategy();
    
    // To help the controller pick a strategy, estimate a commitment time for the strategy.
    public abstract int GetCommitmentTime();
    public int difficulty; // An abstract difficulty level for the strategy.
    public float intensity; // Use this to modify the behavior of the strategy to make it more difficult.
    
    // More polite strategies will yield to less polite strategies.
    public int GetPoliteness()
    {
        // In case I want to put anything that factors politeness for ALL strategies.
        return GetPolitenessInternal();
    }
    protected abstract int GetPolitenessInternal();

    protected GameObject spawnEffect;
    protected Actor Spawn(GameObject gobj, Vector2 point)
    {
        spawnEffect ??= Resources.Load<GameObject>("SpawnEffect");
        Instantiate(spawnEffect, point, Quaternion.identity);
        var actor = Instantiate(gobj, point, Quaternion.identity);
        return actor.GetComponent<Actor>();
    }
}
