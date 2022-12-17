using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropDuster : AttackController
{
    public ActorAttack attack;

    private Vector2 _positionOfLastAttack;
    public float spaceBetweenAttacks;

    protected override void InitializeInternal()
    {
        attack.attacker = owner;
    }

    public override void DoAttack()
    {
        if (Vector2.Distance(transform.position, _positionOfLastAttack) > spaceBetweenAttacks)
        {
            attack.PerformAttack(GetDirectionToTarget());
        }
    }
}
