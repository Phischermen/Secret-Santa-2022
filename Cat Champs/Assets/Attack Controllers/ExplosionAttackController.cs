using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionAttackController : AttackController
{
    public float countdown = 10f;
    private float _internalCountdown;
    public ExplosionAttack explosionAttack;

    protected override void InitializeInternal()
    {
        base.InitializeInternal();
        _internalCountdown = countdown;
    }

    public override void DoAttack()
    {
        if (_internalCountdown <= 0f || InRange(explosionAttack.range))
        {
            explosionAttack.PerformAttack(Vector2.zero);
            _internalCountdown = countdown;
        }
        else
        {
            _internalCountdown -= Time.deltaTime;
        }
    }

    public void JustExplodeDamnIt()
    {
        _internalCountdown = 0f;
        DoAttack();
    }
}
