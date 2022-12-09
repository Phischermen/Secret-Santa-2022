using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [HideInInspector] public Actor attacker;
    public MotionProvider motionProvider;
    public MotionController motionController;
    public ProjectileStats projectileStats;

    [HideInInspector] public bool targetingPlayer;
    
    private void Start()
    {
        motionController.Initialize(transform);
        motionController.GetSpeed = () => projectileStats.speed;
        motionController.GetAcceleration = () => projectileStats.acceleration;
    }

    public void Update()
    {
        motionController.Move(motionProvider.GetMotion());
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (targetingPlayer && col.CompareTag("Player"))
        {
            var playerActor = col.gameObject.GetComponent<PlayerActor>();
            // todo apply debuff
            playerActor.health.TakeDamage((int)(projectileStats.damage * attacker.stats.attackDamageMod));
        }
        else if (col.TryGetComponent(out Actor actor))
        {
            // todo apply debuff
            actor.health.TakeDamage((int)(projectileStats.damage * attacker.stats.attackDamageMod));
        }
    }
}
