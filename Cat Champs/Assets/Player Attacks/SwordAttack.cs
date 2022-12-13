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
    
    public float swingDuration = 0.5f;
    public float growDuration = 0.2f;

    protected override void PerformAttackInternal(Vector2 direction)
    {
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
            .Append(
                transform.DOScale(0f, growDuration)
                    .From(1f))
            .Insert( atPosition: 0f,
                sword.DOLocalRotate(new Vector3(0, 0, arc), swingDuration)
                    .From(new Vector3(0, 0, -arc))
                    .SetRelative(true)
                    .OnComplete(() => { swordSprite.enabled = false; }))
            
            .Play();
        // Perform a circle check to see if we hit anything
        var size = Physics2D.OverlapCircleNonAlloc(transform.position, range, _results);
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
                    // Hit the enemy
                    Debug.Log("Hit " + enemy.name);
                    enemy.health.TakeDamage(damage);
                }
            }
        }
    }
}
