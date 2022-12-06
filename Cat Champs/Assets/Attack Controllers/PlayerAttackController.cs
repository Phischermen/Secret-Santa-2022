using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttackController : AttackController
{
    /* The player has attacks that trigger passively. These attacks run on a timer. */
    public List<ActorAttack> passiveAttacks = new List<ActorAttack>();
    
    /* The player has attacks that they may trigger on command */
    public List<ActorAttack> activeAttacks = new List<ActorAttack>();
    
    private Camera _mainCamera;
    [HideInInspector] public InputAction attack;
    [HideInInspector] public InputAction mouseAim;

    protected override void InitializeInternal()
    {
        _mainCamera = Camera.main;
        foreach (var passiveAttack in passiveAttacks)
        {
            passiveAttack.attacker = owner;
        }

        foreach (var activeAttack in activeAttacks)
        {
            activeAttack.attacker = owner;
        }
    }

    public override void DoAttack()
    {
        // Get the mouse position via the mouseAim input action.
        Vector2 mousePosition = mouseAim.ReadValue<Vector2>();
        // Convert the mouse position to a world position.
        Vector3 worldPosition = _mainCamera.ScreenToWorldPoint(mousePosition);
        // Get the direction from the player to the mouse position.
        Vector3 direction = (worldPosition - owner.transform.position).normalized;
        // Perform passive attacks.
        foreach (ActorAttack passiveAttack in passiveAttacks)
        {
            passiveAttack.PerformAttack(direction);
        }
        if (attack.WasPerformedThisFrame())
        {
            Debug.Log("Attack!");
            foreach (var activeAttack in activeAttacks)
            {
                activeAttack.PerformAttack(direction);
            }
        }
    }
}
