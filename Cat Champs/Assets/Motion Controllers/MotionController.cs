using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MotionController : MonoBehaviour
{
    [HideInInspector] public Actor owner;
    protected float Speed => owner.stats.speed;
    protected float Acceleration => owner.stats.acceleration;
    
    public void Initialize(Actor owner)
    {
        this.owner = owner;
    }
    
    protected void Translate(Vector2 translation) => owner.transform.Translate(new Vector3(translation.x, translation.y, 0));
    
    public void Move(Vector2 direction)
    {
        MoveInternal(direction);
        //todo: resolve collision with edge of arena.
    }
    
    protected abstract void MoveInternal(Vector2 direction); 
}
