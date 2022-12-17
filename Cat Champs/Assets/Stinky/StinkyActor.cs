using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StinkyActor : Actor
{
    protected new void Start()
    {
        base.Start();
        ((BeelineMotionProvider)motionProvider).target = GameplayState.GetPlayer().transform;
        motionController.HitEdge += MotionControllerOnHitEdge;
        health.HealthDepleted += HealthOnHealthDepleted;
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
