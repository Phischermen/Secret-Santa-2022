using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAttack : ActorAttack, IUpgrades
{
    // These stats are applied as temporary debuffs the the hit actor. 
    public GameObject projectile;
    public LayerMask hitMask;
    public int bonusDamage;
    

    private void Start()
    {
        InitializeUpgrades();
    }

    protected override void PerformAttackInternal(Vector2 direction)
    {
        // Spawn the projectile. Make it face the direction of the attack.
        var gobj = Instantiate(projectile, transform.position, Quaternion.LookRotation(Vector3.forward, direction));
        // Get the projectile component and set its owner.
        var proj = gobj.GetComponent<Projectile>();
        proj.attacker = attacker;
        proj.attack = this;
        proj.targetLayer = hitMask;
        if (proj.motionProvider is LinearMotionProvider linear)
        {
            linear.direction = direction;
        }
    }

    private List<Upgrade> _upgrades = new();
    public void InitializeUpgrades()
    {
        _upgrades.Add(new BonusDamageUpgrade(this)
        {
            name = $"Increase damage of {name}",
            description = $"Increase the damage of {name} by 20%",
        });
        _upgrades.Add(new AttackFrequencyUpgrade(this)
        {
            name = $"Increase the frequency of {name}",
            description = $"Decrease the cooldown of {name} by half a second",
        });
    }

    public List<Upgrade> GetUpgrades()
    {
        return _upgrades;
    }
    
    private class BonusDamageUpgrade : Upgrade
    {
        private ProjectileAttack _attack;
        public BonusDamageUpgrade(ProjectileAttack attack)
        {
            _attack = attack;
        }

        public override void UpgradeChosen()
        {
            _attack.bonusDamage = (int)(_attack.bonusDamage + 1 * 1.2f);
        }
    }
    
    private class AttackFrequencyUpgrade : Upgrade
    {
        private ProjectileAttack _attack;
        
        public AttackFrequencyUpgrade(ProjectileAttack attack)
        {
            _attack = attack;
        }
        public override void UpgradeChosen()
        {
            _attack.attackCooldown -= 0.1f;
        }
    }
}
