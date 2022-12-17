using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class ActorAttack : MonoBehaviour
{
    [HideInInspector] public Actor attacker;
    public float FrequencyMod => attacker.GetAttackFrequencyMod();
    [FormerlySerializedAs("attackFrequency")] public float attackCooldown;
    protected float timeOfLastAttack;
    //protected float cooldown = 0;
    protected float timeSinceLastAttack => Time.time - timeOfLastAttack;
    
    public event Action AttackPerformed;

    public void PerformAttack(Vector2 direction)
    {
        var cooldown = attackCooldown * FrequencyMod;
        if (timeSinceLastAttack >= cooldown)
        {
            timeOfLastAttack = Time.time;
            AttackPerformed?.Invoke();
            PerformAttackInternal(direction);
        }
    }
    
    protected abstract void PerformAttackInternal(Vector2 direction);
}
