using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
            if (countdown - _internalCountdown < 1f) return; // Bugfix: Prevents the explosion from being triggered immediately after spawning.
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
