using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpiderActor : Actor
{
    public bool destroyWhenHealthDepleted = true;
    protected new void Start()
    {
        base.Start();
        motionController.HitEdge += MotionController_HitEdge;
        health.HealthDepleted += Health_HealthDepleted;
    }

    private void Health_HealthDepleted(Actor obj)
    {
        if (destroyWhenHealthDepleted)
        {
            Destroy(obj.gameObject);
        }
    }

    private void MotionController_HitEdge(Vector2 hitPosition)
    {
        var arena = GameplayState.GetArena();
        var linearMotionProvider = ((LinearMotionProvider)motionProvider);
        var lerpedMotionController = ((LerpedMotionController)motionController);
        if (arena.IsCollision(transform.position + Vector3.up * 0.5f) ||
            arena.IsCollision(transform.position + Vector3.down * 0.5f))
        {
            linearMotionProvider.direction.y *= -1;
            lerpedMotionController.velocity.y *= -1;
        }
        else if (arena.IsCollision(transform.position + Vector3.left * 0.5f) ||
                 arena.IsCollision(transform.position + Vector3.right * 0.5f))
        {
            linearMotionProvider.direction.x *= -1;
            lerpedMotionController.velocity.x *= -1;
        }
    }
}