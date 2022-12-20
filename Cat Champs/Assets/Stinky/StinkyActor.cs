using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StinkyActor : Actor
{
    protected new void Start()
    {
        base.Start();
        motionController.HitEdge += MotionControllerOnHitEdge;
        health.HealthDepleted += HealthOnHealthDepleted;
        Destroy(gameObject, 20f);
    }

    private void HealthOnHealthDepleted(Actor obj)
    {
        Destroy(obj.gameObject);
    }

    private void MotionControllerOnHitEdge(Vector2 obj)
    {
        var lerperdMotionController = (LerpedMotionController)motionController;
        lerperdMotionController.velocity *= -1;
    }
}
