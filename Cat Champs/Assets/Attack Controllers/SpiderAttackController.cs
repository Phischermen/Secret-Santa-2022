using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderAttackController : AttackController
{
    public ActorAttack webAttack;
    // Ensure only one attack is active at a time.
    public string attackGroup;
    private static Dictionary<string, SpiderAttackController> attackingSpiderOfGroup = new Dictionary<string, SpiderAttackController>();

    
    private float attackRange => owner.stats.attackRangeMod;

    protected override void InitializeInternal()
    {
        base.InitializeInternal();
        webAttack.attacker = owner;
        owner.health.HealthDepleted += _ =>
        {
            QueryAttacker(out var attackerIsMe, out var _);
            if (attackerIsMe)
            {
                ClearAttacker();
            }
        };
    }

    public override void DoAttack()
    {
        QueryAttacker(out bool attackerIsMe, out bool attackerRoleIsTaken);
        bool inRange = false;
        if (!attackerRoleIsTaken)
        {
            // Enemy may be candidate to become attacker.
            inRange = InRange(attackRange);
            if (inRange) BecomeAttacker();
        }

        if (!attackerIsMe) return;
        inRange = inRange || InRange(attackRange);
        if (inRange)
        {
            webAttack.PerformAttack(GetDirectionToTarget());
        }
        else
        {
            // Enemy is no longer in range.
            ClearAttacker();
        }
    }
    
    // Helpers
    private void QueryAttacker(out bool attackerIsMe, out bool attackerRoleIsTaken)
    {
        var attackerExists = attackingSpiderOfGroup.TryGetValue(attackGroup, out var spider);
        attackerIsMe = attackerExists && spider == this;
        attackerRoleIsTaken = attackerExists && spider != null;
    }

    private void BecomeAttacker() => attackingSpiderOfGroup[attackGroup] = this;
    private void ClearAttacker() => attackingSpiderOfGroup[attackGroup] = null;
}
