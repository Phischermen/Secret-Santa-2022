using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Projectile Stats", menuName = "Projectile Stats")]
public class ProjectileStats : ScriptableObject
{
    public float speed;
    public float acceleration;
    public int damage;
    public ActorStats debuff;
    public int maxDebuffStacks;
    public float debuffDuration;
    public float lifespan;
    public bool canBeClearedBySword;
}
