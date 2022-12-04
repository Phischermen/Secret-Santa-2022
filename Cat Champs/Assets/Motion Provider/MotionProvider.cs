using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MotionProvider : MonoBehaviour
{
    [HideInInspector] public Actor owner;
    protected abstract Vector2 GetMotionInternal();
    
    public void Initialize(Actor owner)
    {
        this.owner = owner;
    }

    public Vector2 GetMotion()
    {
        return GetMotionInternal().normalized;
    }
}
