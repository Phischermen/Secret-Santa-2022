using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Actor : MonoBehaviour
{
    // Provides stats for the actor.
    [FormerlySerializedAs("stats")] public ActorStats baseStats;
    [HideInInspector] public List<(ActorStats stats, float expire, GameObject particles)> debuffs = new();

    // Provides the actor's current health, and signals death.
    public HealthManager health;

    // Provides direction to move in.
    public MotionProvider motionProvider;
    
    // Takes care of moving the actor.
    public MotionController motionController;

    // Takes care of attacking.
    public AttackController attackController;

    protected void Start()
    {
        health.Initialize(this);
        motionController.Initialize(transform);
        motionController.GetSpeed = GetSpeed;
        motionController.GetAcceleration = GetAcceleration;
        attackController.Initialize(this);
    }
    

    protected void Update()
    {
        // Get the direction to move in.
        Vector2 direction = motionProvider.GetMotion();
        
        // Move the actor.
        motionController.Move(direction);
        
        // Invoke the attack controller.
        attackController.DoAttack();
        
        // Check if any debuffs have expired.
        for (var i = 0; i < debuffs.Count; i++)
        {
            var valueTuple = debuffs[i];
            if (valueTuple.expire < Time.time)
            {
                Destroy(valueTuple.particles);
                debuffs.RemoveAt(i);
                i--;
            }
        }
    }
    
    public void AddDebuff(ActorStats debuff, float expire, int max, GameObject particles)
    {
        // Instantiate the particles.
        var transform1 = transform;
        GameObject particlesInstance = Instantiate(particles, transform1.position, Quaternion.identity, transform1);
        // Check if the debuff is already applied by counting.
        int count = debuffs.Count(tuple => tuple.stats == debuff);
        if (count < max)
        {
            debuffs.Add((debuff, Time.time + expire, particlesInstance));
        }
    }
    
    public float GetSpeed()
    {
        return baseStats.speed + debuffs.Sum(debuff => debuff.stats.speed);
    }
    
    public float GetAcceleration()
    {
        return baseStats.acceleration + debuffs.Sum(debuff => debuff.stats.acceleration);
    }

    public float GetAttackRangeMod()
    {
        return baseStats.attackRangeMod + debuffs.Sum(debuff => debuff.stats.attackRangeMod);
    }
    
    public float GetAttackFrequencyMod()
    {
        var sum = baseStats.attackCooldownMod + debuffs.Sum(debuff => debuff.stats.attackCooldownMod);
        return sum < 0 ? 0.01f : sum;
    }
    
    public float GetAttackDamageMod()
    {
        return baseStats.attackDamageMod + debuffs.Sum(debuff => debuff.stats.attackDamageMod);
    }
}
