using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class HealthManager : MonoBehaviour
{
    [HideInInspector] public Actor owner;
    public bool useIframes = false;

    public int MaxHealth => owner.baseStats.health;
    
    protected int _iframes = 0;
    public bool iFrameIsEven => _iframes % 2 == 0; // Used to flash the sprite when invulnerable.
    public int CurrentHealth { get; private set; }
    
    public event Action<HealthManager, int> OnDamageTaken;
    public event Action<Actor> HealthDepleted;
    
    public void Initialize(Actor owner)
    {
        this.owner = owner;
        CurrentHealth = MaxHealth;
    }
    
    public void TakeDamage(int damage, ActorStats debuff = null)
    {
        if (_iframes > 0)
            return;
        
        CurrentHealth -= damage;
        OnDamageTaken?.Invoke(this, damage);
        TextPopup.CreateForDamage(damage, 100, (Vector2)owner.transform.position + Random.insideUnitCircle);
        if (useIframes) _iframes = 30;

        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            HealthDepleted?.Invoke(owner);
            HealthDepletedInternal();
        }
    }
    
    public void Heal(int amount)
    {
        CurrentHealth = Mathf.Min(CurrentHealth + amount, MaxHealth);
    }
    
    public void Update()
    {
        if (_iframes > 0)
            _iframes--;
    }
    
    protected virtual void HealthDepletedInternal()
    {
        // Do something when health is depleted.
    }
}
