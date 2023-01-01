using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class ActorAttack : MonoBehaviour
{
    public bool usesMouseTargeting;
    public bool usesMotionTargeting;
    public bool isPassive;
    [HideInInspector] public Actor attacker;
    public float FrequencyMod => attacker.GetAttackFrequencyMod();
    [FormerlySerializedAs("attackFrequency")] public float attackCooldown;
    protected float timeOfLastAttack;
    //protected float cooldown = 0;
    protected float timeSinceLastAttack => Time.time - timeOfLastAttack;
    
    public event Action AttackPerformed;
    
    public AudioClip sound;

    public void PerformAttack(Vector2 direction)
    {
        var cooldown = attackCooldown * FrequencyMod;
        if (timeSinceLastAttack >= cooldown)
        {
            timeOfLastAttack = Time.time;
            if (sound) AudioSource.PlayClipAtPoint(sound, transform.position);
            AttackPerformed?.Invoke();
            PerformAttackInternal(direction);
        }
    }
    
    protected abstract void PerformAttackInternal(Vector2 direction);
}
