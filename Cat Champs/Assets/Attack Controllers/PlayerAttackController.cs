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
    
    public List<ActorAttack> obtainableAttacks = new List<ActorAttack>();
    public event Action<Vector2> PerformedActiveAttack;
    
    private Camera _mainCamera;
    [HideInInspector] public InputAction attack;
    [HideInInspector] public InputAction mouseAim;

    protected override void InitializeInternal()
    {
        _mainCamera = Camera.main;
        target = null;
        foreach (var passiveAttack in passiveAttacks)
        {
            passiveAttack.attacker = owner;
        }

        foreach (var activeAttack in activeAttacks)
        {
            activeAttack.attacker = owner;
        }

        foreach (var obtainableAttack in obtainableAttacks)
        {
            obtainableAttack.attacker = owner;
        }
    }

    public override void DoAttack()
    {
        // Get the mouse position via the mouseAim input action.
        Vector2 mousePosition = mouseAim.ReadValue<Vector2>();
        // Convert the mouse position to a world position.
        Vector3 worldPosition = _mainCamera.ScreenToWorldPoint(mousePosition);
        // Get the direction from the player to the mouse position.
        Vector3 aimDirection = (worldPosition - owner.transform.position).normalized;
        // Get the direction of the player's motion.
        Vector3 motionDirection = ((LerpedMotionController)owner.motionController).velocity.normalized;
        // Perform passive attacks.
        foreach (ActorAttack passiveAttack in passiveAttacks)
        {
            if (passiveAttack.usesMouseTargeting)
            {
                passiveAttack.PerformAttack(aimDirection);
            }
            else if (passiveAttack.usesMotionTargeting)
            {
                passiveAttack.PerformAttack(motionDirection);
            }
            else
            {
                passiveAttack.PerformAttack(Vector2.zero);
            }
        }
        if (attack.WasPerformedThisFrame())
        {
            PerformedActiveAttack?.Invoke(aimDirection);
            foreach (var activeAttack in activeAttacks)
            {
                if (activeAttack.usesMouseTargeting)
                {
                    activeAttack.PerformAttack(aimDirection);
                }
                else if (activeAttack.usesMotionTargeting)
                {
                    activeAttack.PerformAttack(motionDirection);
                }
                else
                {
                    activeAttack.PerformAttack(Vector2.zero);
                }
            }
        }
    }

    public List<Upgrade> GetUpgradesFromAllAttacks()
    {
        var upgrades = new List<Upgrade>();
        foreach (var activeAttack in activeAttacks)
        {
            if (activeAttack is IUpgrades upgrades1)
            {
                upgrades.AddRange(upgrades1.GetUpgrades());
            }
        }

        foreach (var passiveAttack in passiveAttacks)
        {
            if (passiveAttack is IUpgrades upgrades1)
            {
                upgrades.AddRange(upgrades1.GetUpgrades());
            }
        }

        foreach (var obtainableAttack in obtainableAttacks)
        {
            var listToAddTo = obtainableAttack.isPassive ? passiveAttacks : activeAttacks;
            upgrades.Add(new ObtainWeapon(obtainableAttack, listToAddTo, obtainableAttacks)
            {
                name = "Unlock " + obtainableAttack.name
            });
        }
        return upgrades;
    }
    
    private class ObtainWeapon : Upgrade
    {
        public ObtainWeapon(ActorAttack attack, List<ActorAttack> listToAddTo, List<ActorAttack> listToRemoveFrom)
        {
            _attack = attack;
            _listToAddTo = listToAddTo;
            _listToRemoveFrom = listToRemoveFrom;
        }
        private ActorAttack _attack;
        private List<ActorAttack> _listToAddTo;
        private List<ActorAttack> _listToRemoveFrom;
        public override void UpgradeChosen()
        {
            _listToAddTo.Add(_attack);
            _listToRemoveFrom.Remove(_attack);
        }
    }
}
