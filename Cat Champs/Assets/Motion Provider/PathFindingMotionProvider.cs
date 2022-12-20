using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFindingMotionProvider : MotionProvider
{
    protected override Vector2 GetMotionInternal()
    {
        return GameplayState.GetArena().GetMotionTowardsTarget(transform.position);
    }
}
