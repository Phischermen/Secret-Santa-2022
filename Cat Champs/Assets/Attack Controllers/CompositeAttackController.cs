using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompositeAttackController : AttackController
{
    public List<AttackController> attacks;

    protected override void InitializeInternal()
    {
        base.InitializeInternal();
        foreach (var attackController in attacks)
        {
            attackController.Initialize(owner);
        }
    }

    public override void DoAttack()
    {
        foreach (AttackController attack in attacks)
        {
            attack.DoAttack();
        }
    }
}
