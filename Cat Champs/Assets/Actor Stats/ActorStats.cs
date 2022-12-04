using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Actor Stats", menuName = "Actor Stats")]
public class ActorStats : ScriptableObject
{
    public int health;
    public float speed;
    public float acceleration;
}
