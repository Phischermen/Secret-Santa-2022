using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpedMotionController : MotionController
{
    /* Movement is implemented with velocity that is lerped to the target velocity. */
    public bool slideAgainstWalls;
    
    public Vector2 velocity;
    public Vector2 TargetVelocity { get; private set; }
    protected override void MoveInternal(Vector2 direction)
    {
        TargetVelocity = direction * GetSpeed.Invoke();
        velocity = Vector2.Lerp(velocity, TargetVelocity, Time.deltaTime * GetAcceleration.Invoke());
        if (slideAgainstWalls)
        {
            var position = transform.position;
            var intersects = GameplayState.GetArena().IsCollisionExt(position, (Vector2)position + (velocity * Time.deltaTime), out var intersectionPoint, out var resolving);
            if (intersects)
            {
                Translate(velocity * resolving * Time.deltaTime);
            }
            else
            {
                Translate(velocity * Time.deltaTime);
            }
            // Clamp postition to arena bounds
            var pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, GameplayState.GetArena().arenaBounds.min.x + 0.1f, GameplayState.GetArena().arenaBounds.max.x - 0.1f);
            pos.y = Mathf.Clamp(pos.y, GameplayState.GetArena().arenaBounds.min.y + 0.1f, GameplayState.GetArena().arenaBounds.max.y - 0.1f);
            transform.position = pos;
        }
        else
        {
            Translate(velocity * Time.deltaTime);
        }
        
    }
}
