using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActorAttack : MonoBehaviour
{
    [HideInInspector] public Actor attacker;
    public float FrequencyMod => attacker.stats.attackFrequencyMod;
    public float attackFrequency;
    protected float cooldown = 0;
    
    public event Action AttackPerformed;

    public void PerformAttack(Vector2 direction)
    {
        if (cooldown <= 0)
        {
            cooldown = attackFrequency * FrequencyMod;
            AttackPerformed?.Invoke();
            PerformAttackInternal(direction);
        }
    }
    
    protected abstract void PerformAttackInternal(Vector2 direction);

    private void Update()
    {
        // Decrement cooldown by time since last frame
        cooldown -= Time.deltaTime;
    }
}
