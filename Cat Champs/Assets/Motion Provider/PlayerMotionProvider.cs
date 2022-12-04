using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotionProvider : MotionProvider
{
    PlayerControls _controls;
    Vector2 _move;

    private void Awake()
    {
        _controls = new PlayerControls();
        _controls.Enable();
        // _controls.Gameplay.Movement. += ctx => _move = ctx.ReadValue<Vector2>();
    }

    protected override Vector2 GetMotionInternal()
    {
        // if (_controls.Gameplay.Movement.IsPressed())
        // {
        //     _controls.Gameplay.Movement.ReadValue<Vector2>();
        // }
        return _controls.Gameplay.Movement.ReadValue<Vector2>();
    }
}
