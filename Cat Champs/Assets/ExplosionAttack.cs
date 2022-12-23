using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ExplosionAttack : ActorAttack, IUpgrades
{
    public bool destroyObjectWhenAnimationFinished;
    public float range = 1f;
    public float damage = 1f;
    protected Collider2D[] _results = new Collider2D[10];
    public LayerMask layerMask;

    public GameObject objectToDestroy;
    public GameObject explosionParticles;
    public Transform shockwaveContainer;
    public SpriteRenderer shockwave;

    public float growDuration;
    public float fadeOutTime;

    private void Start()
    {
        InitializeUpgrades();
    }

    protected override void PerformAttackInternal(Vector2 direction)
    {
        Debug.Log("Performing attack");
        // Animate the explosion with a particle system and a sprite.
        // Make sprite grow fast to the explosion attack's radius then fade slowly.
        shockwave.color = Color.white;
        DOTween.Kill(this);
        var sequence = DOTween.Sequence(this)
            .Append(
                shockwaveContainer.DOScale(range, growDuration)
                    .From(0f))
            .Append(
                shockwave.DOFade(0f, fadeOutTime)
                    .SetEase(Ease.OutQuad));
        if (destroyObjectWhenAnimationFinished)
        {
            sequence.OnComplete(() => Destroy(objectToDestroy));
        }

        sequence.Play();
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
            explosionAttack.range += 1f;
        }
    }
    
    protected class DamageUpgrade : Upgrade
    {
        public ExplosionAttack explosionAttack;
        public override void UpgradeChosen()
        {
            explosionAttack.damage += 10f;
        }
    }

}
