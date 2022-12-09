using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAttack : ActorAttack
{
    // These stats are applied as temporary debuffs the the hit actor. 
    public GameObject projectile;
    protected override void PerformAttackInternal(Vector2 direction)
    {
        // Spawn the projectile. Make it face the direction of the attack.
        var gobj = Instantiate(projectile, transform.position, Quaternion.LookRotation(Vector3.forward, direction));
        // Get the projectile component and set its owner.
        var proj = gobj.GetComponent<Projectile>();
        proj.attacker = attacker;
        proj.targetingPlayer = attacker.attackController.target == GameplayState.GetPlayer();
        if (proj.motionProvider is LinearMotionProvider linear)
        {
            linear.direction = direction;
        }
    }
}
