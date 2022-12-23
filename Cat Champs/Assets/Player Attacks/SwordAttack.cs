using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SwordAttack : ActorAttack, IUpgrades
{
    public float range = 1.0f;
    public float arc = 45.0f;
    public int damage = 10;
    protected Collider2D[] _results = new Collider2D[100];
    public LayerMask layerMask;

    public Transform sword;
    public SpriteRenderer swordSprite;

    public float swingDuration = 0.5f;
    public float growDuration = 0.2f;
    
    public TrailRenderer trail;

    private void Start()
    {
        InitializeUpgrades();
        trail.time = swingDuration;
    }

    protected override void PerformAttackInternal(Vector2 direction)
    {
        trail.emitting = true;
        trail.widthMultiplier = range * 0.5f;
        trail.transform.localPosition = new Vector3(0, range * 0.5f, 0);
        // Animate the sword with DoTween.
        swordSprite.enabled = true;
        // Rotate to the direction of the attack.
        transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
        // Swing the sword to cover the arc.
        DOTween.Kill(this);
        DOTween.Sequence(this)
            .Append(
                transform.DOScale(1f, growDuration)
                    .From(0f))
            .AppendInterval(swingDuration - growDuration * 2)
            .Append(
                transform.DOScale(0f, growDuration)
                    .From(1f))
            .Insert(atPosition: 0f,
                sword.DOLocalRotate(new Vector3(0, 0, arc * 2), swingDuration, RotateMode.LocalAxisAdd)
                    .From(new Vector3(0, 0, -arc))
                    .SetRelative(true)
                    .OnComplete(() => { swordSprite.enabled = trail.emitting = false; }))
            .Play();
        // Perform a circle check to see if we hit anything
        var size = Physics2D.OverlapCircleNonAlloc(transform.position, range, _results, layerMask);
        for (int i = 0; i < size; i++)
        {
            var hit = _results[i];
            // Check if the hit object is an enemy
            var enemy = hit.GetComponent<Actor>();
            if (enemy != null && enemy != attacker)
            {
                // Check if the enemy is within the arc
                var angle = Vector2.Angle(direction, hit.transform.position - transform.position);
                if (angle < arc)
                {
                    Debug.DrawLine(transform.position, hit.transform.position, Color.green, 1f);
                    // Hit the enemy
                    //Debug.Log("Hit " + enemy.name);
                    enemy.health.TakeDamage((int)(attacker.GetAttackDamageMod() * damage));
                }
                else
                {
                    Debug.DrawLine(transform.position, hit.transform.position, Color.red, 1f);
                    //Debug.Log("Missed " + angle);
                }
            }
            else
            {
                // Check if the hit object is a projectile
                var projectile = hit.GetComponent<Projectile>();
                if (projectile != null)
                {
                    // Check if the projectile is within the arc
                    var angle = Vector2.Angle(direction, hit.transform.position - transform.position);
                    if (angle < arc && projectile.projectileStats.canBeClearedBySword && projectile.attacker != attacker)
                    {
                        Destroy(projectile.gameObject);
                    }
                }
            }
        }
    }

    private List<Upgrade> _upgrades = new List<Upgrade>();

    public void InitializeUpgrades()
    {
        _upgrades.Add(new DamageUpgrade()
        {
            name = "Increase Sword Damage",
            description = "Increase the damage of the sword by 20%",
            swordAttack = this
        });
        _upgrades.Add(new RangeUpgrade()
        {
            name = "Increase Sword Range",
            description = "Increase the range of the sword by half a meter",
            swordAttack = this
        });
        _upgrades.Add(new ArcUpgrade()
        {
            name = "Increase Sword Arc",
            description = "Increase the arc of the sword by 10 degrees",
            swordAttack = this
        });
    }
    public List<Upgrade> GetUpgrades()
    {
        return _upgrades;
    }

    protected class RangeUpgrade : Upgrade
    {
        public SwordAttack swordAttack;

        public override void UpgradeChosen()
        {
            swordAttack.range += 0.5f;
        }
    }

    protected class DamageUpgrade : Upgrade
    {
        public SwordAttack swordAttack;

        public override void UpgradeChosen()
        {
            swordAttack.damage = (int)(swordAttack.damage * 1.2f);
        }
    }
    
    protected class ArcUpgrade : Upgrade
    {
        public SwordAttack swordAttack;

        public override void UpgradeChosen()
        {
            swordAttack.arc += 10f;
        }
    }

    
}