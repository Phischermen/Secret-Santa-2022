using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActor : Actor
{
    PlayerControls _controls;

    public PlayerAnimator playerAnimator;

    private void Start()
    {
        _controls = new PlayerControls();
        _controls.Enable();

        ((PlayerMotionProvider)motionProvider).movement = _controls.Gameplay.Movement;
        ((PlayerAttackController)attackController).attack = _controls.Gameplay.Attack;
        ((PlayerAttackController)attackController).mouseAim = _controls.Gameplay.MouseAim;

        _controls.Gameplay.Movement.performed += ctx => playerAnimator.FlipBasedOnDirection(ctx.ReadValue<Vector2>());
        ((PlayerAttackController)attackController).PerformedActiveAttack +=
            direction => playerAnimator.FlipBasedOnDirection(direction);
    }
}