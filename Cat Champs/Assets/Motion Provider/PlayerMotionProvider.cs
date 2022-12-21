using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMotionProvider : MotionProvider
{
    [HideInInspector] public InputAction movement;

    protected override Vector2 GetMotionInternal()
    {
        var desiredMotion = movement.ReadValue<Vector2>();
        return desiredMotion;
    }
}
