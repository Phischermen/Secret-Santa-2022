using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MotionController : MonoBehaviour
{
    [HideInInspector] public Transform owner;
    public Func<float> GetSpeed = () => 1f;
    public Func<float> GetAcceleration = () => 1f;
    public event Action<Vector2> HitEdge;
    
    public void Initialize(Transform owner)
    {
        this.owner = owner;
    }
    
    protected void Translate(Vector2 translation) => owner.transform.Translate(new Vector3(translation.x, translation.y, 0), Space.World);
    
    public void Move(Vector2 direction)
    {
        // Cache current positon
        var currentPosition = owner.position;
        MoveInternal(direction);
        if (GameplayState.GetArena().IsCollision(owner.position))
        {
            var hitPosition = owner.position;
            // If we hit a wall, move back to the previous position
            owner.position = currentPosition;
            HitEdge?.Invoke(hitPosition);
        }
    }
    
    protected abstract void MoveInternal(Vector2 direction); 
}
