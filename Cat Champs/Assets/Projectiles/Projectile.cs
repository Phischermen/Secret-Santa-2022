using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [HideInInspector] public Actor attacker;
    public MotionProvider motionProvider;
    public MotionController motionController;
    public ProjectileStats projectileStats;
    public GameObject debuffParticles;

    //[HideInInspector] public bool targetingPlayer;
    public LayerMask targetLayer;
    
    private Collider2D _myCollider;

    private void Awake()
    {
        _myCollider = GetComponent<Collider2D>();
    }

    private void Start()
    {
        motionController.Initialize(transform);
        motionController.GetSpeed = () => projectileStats.speed;
        motionController.GetAcceleration = () => projectileStats.acceleration;
        motionController.HitEdge += _ => Destroy(gameObject);
        
        Destroy(gameObject, projectileStats.lifespan);
    }

    public void Update()
    {
        motionController.Move(motionProvider.GetMotion());
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!_myCollider.IsTouchingLayers(targetLayer)) return;
        if (col.TryGetComponent(out Actor actor))
        {
            actor.AddDebuff(projectileStats.debuff, projectileStats.debuffDuration, projectileStats.maxDebuffStacks, debuffParticles);
            actor.health.TakeDamage((int)(projectileStats.damage * attacker.GetAttackDamageMod()));
            Destroy(gameObject);
        }
    }
}
