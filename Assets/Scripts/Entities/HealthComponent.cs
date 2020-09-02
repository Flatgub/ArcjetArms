using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// HealthComponent is used to keep track of the health of entities, such as the player, enemies
/// and obstructions in the world.
/// </summary>
public class HealthComponent : MonoBehaviour
{
    public int MaxHealth {get; private set;}
    public int Health {get; private set;}
    public bool IsDead { get; private set; }

    /// <summary>
    /// OnDeath is the event which is triggered the first time an entity reaches zero health
    /// </summary>
    public event Action OnDeath;

    public void ApplyDamage(int amount)
    {
        if (!IsDead)
        {
            Health -= Math.Max(amount,0); //consider using ApplyHealing when damage is negative?
            if (Health <= 0)
            {
                Health = 0;
                OnDeath?.Invoke();
                IsDead = true;
            }
        }
    }

    public void ApplyHealing(int amount)
    {
        amount = Math.Max(amount, 0); //force amount to be positive;
        Health = Math.Min(Health + amount, MaxHealth);
    }

    public float HealthAsFraction()
    {
        return Health / (float)MaxHealth;
    }

    public void SetMaxHealth(int newValue, bool updateHealth = false)
    {
        float beforeFrac = HealthAsFraction();
        MaxHealth = Math.Max(0, newValue);
        if (updateHealth)
        {
            Health = (int)Math.Ceiling(MaxHealth * beforeFrac);
        }
        else
        {
            Health = Math.Min(Health, MaxHealth);
        }
    }

    public void SetHealth(int newValue)
    {
        Health = Math.Max(0, Math.Min(MaxHealth, newValue));
        if (Health == 0)
        {
            OnDeath?.Invoke();
            IsDead = true;
        }
    }

    public static HealthComponent AddHealthComponent(GameObject go, int maxHealth)
    {
        HealthComponent comp = go.AddComponent<HealthComponent>();
        comp.SetMaxHealth(maxHealth);
        comp.SetHealth(maxHealth);
        return comp;
    }
}
