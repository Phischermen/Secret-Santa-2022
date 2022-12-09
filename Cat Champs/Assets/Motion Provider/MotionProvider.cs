using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MotionProvider : MonoBehaviour
{
    protected abstract Vector2 GetMotionInternal();

    public Vector2 GetMotion()
    {
        return GetMotionInternal().normalized;
    }
}
