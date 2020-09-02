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
    public int Current {get; private set;}
    public bool IsDead { get; private set; }

    /// <summary>
    /// OnDeath is the event which is triggered the first time an entity reaches zero health
    /// </summary>
    public event Action OnDeath;

    public void ApplyDamage(int amount)
    {
        if (!IsDead)
        {
            Current -= Math.Max(amount,0); //consider using ApplyHealing when damage is negative?
            if (Current <= 0)
            {
                Current = 0;
                OnDeath?.Invoke();
                IsDead = true;
            }
        }
    }

    public void ApplyHealing(int amount)
    {
        amount = Math.Max(amount, 0); //force amount to be positive;
        Current = Math.Min(Current + amount, MaxHealth);
    }

    public float HealthAsFraction()
    {
        return Current / (float)MaxHealth;
    }

    public void SetMaxHealth(int newValue, bool updateHealth = false)
    {
        float beforeFrac = HealthAsFraction();
        MaxHealth = Math.Max(0, newValue);
        if (updateHealth)
        {
            Current = (int)Math.Ceiling(MaxHealth * beforeFrac);
        }
        else
        {
            Current = Math.Min(Current, MaxHealth);
        }
    }

    public void SetHealth(int newValue)
    {
        Current = Math.Max(0, Math.Min(MaxHealth, newValue));
        if (Current == 0)
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
