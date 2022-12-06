using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SwordAttack : ActorAttack
{
    public float range = 1.0f;
    public float arc = 45.0f;
    public int damage = 10;
    protected Collider2D[] _results = new Collider2D[10];
    
    public Transform sword;
    public SpriteRenderer swordSprite;

    protected override void PerformAttackInternal(Vector2 direction)
    {
        // Animate the sword with DoTween.
        swordSprite.enabled = true;
        // Rotate to the direction of the attack.
        transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);
        // Swing the sword to cover the arc.
        DOTween.Sequence(this)
            .Append(
                transform.DOScale(1f, 0.05f)
                    .From(0f))
            .Join(
                sword.DOLocalRotate(new Vector3(0, 0, arc), 0.1f)
                    .From(new Vector3(0, 0, -arc))
                    .SetRelative(true)
                    .OnComplete(() => { swordSprite.enabled = false; }))
            .Append(
                transform.DOScale(0f, 0.05f)
                    .From(1f))
            .Play();
        // Perform a circle check to see if we hit anything
        var size = Physics2D.OverlapCircleNonAlloc(transform.position, range, _results);
        for (int i = 0; i < size; i++)
        {
            var hit = _results[i];
            // Check if the hit object is an enemy
            var enemy = hit.GetComponent<Actor>();
            if (enemy != null)
            {
                // Check if the enemy is within the arc
                var angle = Vector2.Angle(direction, hit.transform.position - transform.position);
                if (angle < arc)
                {
                    // Hit the enemy
                    enemy.health.TakeDamage(damage);
                }
            }
        }
    }
}
