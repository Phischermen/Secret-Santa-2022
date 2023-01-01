using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class HealthManager : MonoBehaviour
{
    private float _defense = 1f;
    public float Defense {
        get
        {
            return _defense;
        }
        set{
            _defense = Mathf.Max(value, 1f);
        }
}
    [HideInInspector] public Actor owner;
    public bool useIframes = false;

    public int MaxHealth => owner.baseStats.health;
    
    protected int _iframes = 0;
    public bool iFrameIsEven => _iframes % 2 == 0; // Used to flash the sprite when invulnerable.
    public int CurrentHealth { get; private set; }
    
    public event Action<HealthManager, int> OnDamageTaken;
    public event Action<Actor> HealthDepleted;

    public AudioClip hurtSound;
    public AudioClip deathSound;
    
    public void Initialize(Actor owner)
    {
        this.owner = owner;
        CurrentHealth = MaxHealth;
    }
    
    public void TakeDamage(int damage, ActorStats debuff = null)
    {
        if (_iframes > 0)
            return;

        var damageNotDefended = (int)(damage / Defense);
        CurrentHealth -= damageNotDefended;
        OnDamageTaken?.Invoke(this, damageNotDefended);
        TextPopup.CreateForDamage(damageNotDefended, 100, (Vector2)owner.transform.position + Random.insideUnitCircle);
        if (useIframes) _iframes = 30;

        if (CurrentHealth <= 0)
        {
            if (deathSound) AudioSource.PlayClipAtPoint(deathSound, transform.position);
            CurrentHealth = 0;
            HealthDepleted?.Invoke(owner);
            HealthDepletedInternal();
        }
        else
        {
            if (hurtSound) AudioSource.PlayClipAtPoint(hurtSound, transform.position);
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
