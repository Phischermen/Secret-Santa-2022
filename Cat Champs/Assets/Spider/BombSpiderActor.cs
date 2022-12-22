using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombSpiderActor : SpiderActor
{
    public ActorAttack kamikazeeAttack;
    public SpriteRenderer spriteRenderer;
    protected new void Start()
    {
        base.Start();
        health.HealthDepleted += HealthOnHealthDepleted;
        kamikazeeAttack.attacker = this;
        kamikazeeAttack.AttackPerformed += KamikazeeAttackOnAttackPerformed;
    }

    private void KamikazeeAttackOnAttackPerformed()
    {
        Destroy(this); // This should make the spider stop moving/ attacking.
        spriteRenderer.enabled = false;
    }

    private void HealthOnHealthDepleted(Actor obj)
    {
        kamikazeeAttack.PerformAttack(Vector2.zero);
    }
}
