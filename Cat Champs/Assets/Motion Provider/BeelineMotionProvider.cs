using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeelineMotionProvider : MotionProvider
{
    public Transform target;

    protected override Vector2 GetMotionInternal()
    {
        return target.transform.position - transform.position;
    }
}
