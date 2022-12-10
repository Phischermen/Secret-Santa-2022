using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpedMotionController : MotionController
{
    /* Movement is implemented with velocity that is lerped to the target velocity. */
    public Vector2 velocity;
    public Vector2 TargetVelocity { get; private set; }
    protected override void MoveInternal(Vector2 direction)
    {
        TargetVelocity = direction * GetSpeed.Invoke();
        velocity = Vector2.Lerp(velocity, TargetVelocity, Time.deltaTime * GetAcceleration.Invoke());
        Translate(velocity * Time.deltaTime);
    }
}
