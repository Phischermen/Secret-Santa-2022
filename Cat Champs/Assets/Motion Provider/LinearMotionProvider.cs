using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearMotionProvider : MotionProvider
{
    public Vector2 direction;
    // Provides a linear motion for the object
    protected override Vector2 GetMotionInternal()
    {
        return direction;
    }
}
