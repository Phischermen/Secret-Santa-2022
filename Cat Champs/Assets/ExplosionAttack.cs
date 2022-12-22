using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionAttack : ActorAttack, IUpgrades
{
    public float range = 1f;
    public float damage = 1f;
    protected Collider2D[] _results = new Collider2D[10];
    public LayerMask layerMask;

    public GameObject explosionParticles;

    private void Start()
    {
        InitializeUpgrades();
    }

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

    private List<Upgrade> _upgrades = new();
    public void InitializeUpgrades()
    {
        _upgrades.Add(new DamageUpgrade()
        {
            name = "Increase Explosion Damage",
            description = "Increase the base damage of the explosion by 1",
            explosionAttack = this
        });
        _upgrades.Add(new RangeUpgrade()
        {
            name = "Increase Explosion Range",
            description = "Increase the base range of the explosion by half a meter",
            explosionAttack = this
        });
    }

    public List<Upgrade> GetUpgrades()
    {
        return _upgrades;
    }
    
    protected class RangeUpgrade : Upgrade
    {
        public ExplosionAttack explosionAttack;
        public override void UpgradeChosen()
        {
            explosionAttack.range += 0.5f;
        }
    }
    
    protected class DamageUpgrade : Upgrade
    {
        public ExplosionAttack explosionAttack;
        public override void UpgradeChosen()
        {
            explosionAttack.damage += 1f;
        }
    }

}
