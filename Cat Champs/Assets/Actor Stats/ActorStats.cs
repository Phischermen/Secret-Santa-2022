using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Actor Stats", menuName = "Actor Stats")]
public class ActorStats : ScriptableObject
{
    public int health;
    public float speed;
    public float acceleration;
    [FormerlySerializedAs("attackFrequencyMod")] public float attackCooldownMod = 1f;
    public float attackDamageMod = 1f;
    public float attackRangeMod = 1f;
}
