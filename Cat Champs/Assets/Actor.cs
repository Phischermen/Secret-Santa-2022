using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Actor : MonoBehaviour
{
    // Provides stats for the actor.
    public ActorStats stats;
    
    // Provides the actor's current health, and signals death.
    public HealthManager health;

    // Provides direction to move in.
    public MotionProvider motionProvider;
    
    // Takes care of moving the actor.
    public MotionController motionController;

    private void Start()
    {
        health.Initialize(this);
        motionProvider.Initialize(this);
        motionController.Initialize(this);
    }

    protected void Update()
    {
        // Get the direction to move in.
        Vector2 direction = motionProvider.GetMotion();
        
        // Move the actor.
        motionController.Move(direction);
    }
}
