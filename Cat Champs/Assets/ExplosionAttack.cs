using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionAttack : ActorAttack
{
    public float range = 1f;
    public float damage = 1f;
    protected Collider2D[] _results = new Collider2D[10];
    public LayerMask layerMask;

    public GameObject explosionParticles;
    protected override void PerformAttackInternal(Vector2 direction)
    {
        // Animate the explosion with a particle system
        Instantiate(explosionParticles, transform.position, Quaternion.identity);
        // Perform a circle cast to find all actors in range.
        var size = Physics2D.OverlapCircleNonAlloc(transform.position, range, _results, layerMask);
        for (int i = 0; i < size; i++)
        {
            var actor = _results[i].GetComponent<Actor>();
            if (actor != null)
            {
                actor.health.TakeDamage((int)(damage * attacker.GetAttackDamageMod()));
            }
        }
    }
}
