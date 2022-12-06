using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackController : MonoBehaviour
{
    [HideInInspector] public Actor owner;
    
    public abstract void DoAttack();

    public void Initialize(Actor actor)
    {
        owner = actor;
        InitializeInternal();
    }
    
    protected virtual void InitializeInternal()
    {
        // Do nothing
    }
}
