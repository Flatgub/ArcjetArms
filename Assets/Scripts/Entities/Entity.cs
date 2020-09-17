﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// EntityBase prepresents any entity which might exist on the hex grid, be it a player or an
/// enemy, all code and properties which are common among all entities are shared here.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(HealthComponent))]
public class Entity : MonoBehaviour
{
    public string entityName = "Unnamed";

    public HexGrid Grid { get; private set; }
    public Hex Position { get; private set; }
    public SpriteRenderer appearance;

    public HealthComponent Health { get; private set;}
    public EntityAIController AIController { get; private set;}

    private List<StatusEffect> statusEffects;

    public void Initialize()
    {
        statusEffects = new List<StatusEffect>();
        appearance = GetComponent<SpriteRenderer>();
        Health = GetComponent<HealthComponent>();
        Health.OnDeath += Die;
    }

    public void SetAIController(EntityAIController controller)
    {
        AIController = controller;
    }

    public void AddToGrid(HexGrid grid, Hex pos)
    {
        this.Grid = grid;
        MoveTo(pos);
        grid.AddEntityToGrid(this);
    }

    public void MoveTo(Hex pos)
    {
        if (pos == null)
        {
            throw new ArgumentNullException("Cannot move to null position");
        }
        Position = pos;
        //transform.position = grid.GetWorldPosition(pos);
        //TODO: Calculate travel time using speed somehow?
        LeanTween.moveLocal(gameObject, Grid.GetWorldPosition(pos), 0.1f);
    }

    public void ReceiveDamage(Entity attacker, int damage)
    {
        Health.ApplyDamage(damage);
    }

    public void DealDamageTo(Entity victim, int baseDamage)
    {
        int realDamage = CalculateDamage(baseDamage);
        victim.ReceiveDamage(this, realDamage);
    }

    public void Die()
    {
        if (Grid is HexGrid)
        {
            Grid.RemoveEntityFromGrid(this);
        }
    }

    public void ApplyStatusEffect(StatusEffect effect)
    {
        statusEffects.Add(effect);
    }

    public void RemoveStatusEffect(StatusEffect effect)
    {
        statusEffects.Remove(effect);
    }

    public IEnumerator<StatusEffect> GetStatusEffects()
    {
        return statusEffects.GetEnumerator();
    }

    public void StartTurn(GameplayContext gc)
    {
        //we iterate the loop backwards because statuses might destroy themselves during the loop
        for (int i = statusEffects.Count - 1; i >= 0; i--)
        {
            StatusEffect effect = statusEffects[i];
            if (effect is IStatusTurnStartEventHandler startResponder)
            {
                startResponder.OnTurnStart(this, gc);
            }
        }
    }

    public void EndTurn(GameplayContext gc)
    {
        //we iterate the loop backwards because statuses might destroy themselves during the loop
        for (int i = statusEffects.Count - 1; i >= 0; i--)
        {
            StatusEffect effect = statusEffects[i];
            if (effect is IStatusTurnEndEventHandler endResponder)
            {
                endResponder.OnTurnEnd(this, gc);
            }
        }
    }

    public void TriggerAttackEvent(Entity target, GameplayContext gc)
    {
        //we iterate the loop backwards because statuses might destroy themselves during the loop
        for (int i = statusEffects.Count - 1; i >= 0; i--)
        {
            StatusEffect effect = statusEffects[i];
            if (effect is IStatusAttackEventHandler attackResponder)
            {
                attackResponder.OnAttack(this, target, gc);
            }
        }
    }

    public int CalculateDamage(int baseDamage)
    {
        int result = baseDamage;
        //Additive pass, only + and - operations are used here
        foreach (StatusEffect e in statusEffects)
        {
            if (e is IStatusCalculateDamageEventHandler calculator)
            {
                result = calculator.OnCalculateDamageAdditive(result);
            }
        }

        float floatResult = result;
        //Multiplicative pass, all calculators apply multiplication together
        foreach (StatusEffect e in statusEffects)
        {
            if (e is IStatusCalculateDamageEventHandler calculator)
            {
                floatResult = calculator.OnCalculateDamageMultiplicative(floatResult);
            }
        }

        //always round up
        result = Mathf.CeilToInt(floatResult);
        Debug.Log("input: " + baseDamage + ", output: " + result);
        return result;
    }
}
