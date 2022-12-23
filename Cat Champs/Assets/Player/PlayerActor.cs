using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerActor : Actor
{
    PlayerControls _controls;

    public PlayerAnimator playerAnimator;

    protected new void Start()
    {
        base.Start();
        _controls = new PlayerControls();
        _controls.Enable();

        ((PlayerMotionProvider)motionProvider).movement = _controls.Gameplay.Movement;
        ((PlayerAttackController)attackController).attack = _controls.Gameplay.Attack;
        ((PlayerAttackController)attackController).mouseAim = _controls.Gameplay.MouseAim;

        _controls.Gameplay.Movement.performed += ctx => playerAnimator.FlipBasedOnDirection(ctx.ReadValue<Vector2>());
        ((PlayerAttackController)attackController).PerformedActiveAttack +=
            direction => playerAnimator.FlipBasedOnDirection(direction);
    }

    protected new void Update()
    {
        base.Update();
        playerAnimator.spriteRenderer.enabled = health.iFrameIsEven;
    }

    public event Action<int> XpChanged;
    public event Action<PlayerActor> LevelUp;
    public int level = 1;
    public int experience = 0;
    private int _experienceToNextLevel = 100;

    public int GetExperienceToNextLevel()
    {
        return 100 + level * 10;
    }
    public void CollectExperience(int points)
    {
        experience += points;
        if (experience >= GetExperienceToNextLevel())
        {
            experience -= GetExperienceToNextLevel();
            level++;
            LevelUp?.Invoke(this);
        }
        XpChanged?.Invoke(experience);
    }
}