using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombSpiderActor : SpiderActor
{
    public ActorAttack kamikazeeAttack;
    
    protected new void Start()
    {
        base.Start();
        health.HealthDepleted += HealthOnHealthDepleted;
        kamikazeeAttack.attacker = this;
        kamikazeeAttack.AttackPerformed += KamikazeeAttackOnAttackPerformed;
    }

    private void KamikazeeAttackOnAttackPerformed()
    {
        Destroy(gameObject);
    }

    private void HealthOnHealthDepleted(Actor obj)
    {
        kamikazeeAttack.PerformAttack(Vector2.zero);
    }
}
