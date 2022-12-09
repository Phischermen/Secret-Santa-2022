using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackController : MonoBehaviour
{
    [HideInInspector] public Actor owner;
    [HideInInspector] public Actor target;
    
    public abstract void DoAttack();

    public void Initialize(Actor actor)
    {
        owner = actor;
        target = GameplayState.GetPlayer();
        InitializeInternal();
    }
    
    protected virtual void InitializeInternal()
    {
        // Do nothing
    }
    
    protected Vector2 GetDirectionToTarget()
    {
        return (target.transform.position - transform.position).normalized;
    }

    protected bool InRange(float range)
    {
        return Vector3.Distance(target.transform.position, transform.position) <= range;
    }
}
