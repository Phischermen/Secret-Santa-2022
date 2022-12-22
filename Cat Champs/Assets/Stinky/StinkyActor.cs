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
        //Destroy(gameObject, 20f);
    }

    private void HealthOnHealthDepleted(Actor obj)
    {
        Destroy(obj.gameObject);
    }

    private void MotionControllerOnHitEdge(Vector2 obj)
    {
        var arena = GameplayState.GetArena();
        var lerpedMotionController = ((LerpedMotionController)motionController);
        if (arena.IsCollision(transform.position + Vector3.up * 0.5f) ||
            arena.IsCollision(transform.position + Vector3.down * 0.5f))
        {
            lerpedMotionController.velocity.y *= -1;
        }
        else if (arena.IsCollision(transform.position + Vector3.left * 0.5f) ||
                 arena.IsCollision(transform.position + Vector3.right * 0.5f))
        {
            lerpedMotionController.velocity.x *= -1;
        }
    }
}
