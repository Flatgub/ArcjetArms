﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverclockStatusEffect : StatusEffect, IStatusCalculateDamageEventHandler,
    IStatusTurnEndEventHandler, IStatusApplyEventHandler
{
    //TODO: come back and fix this once resistance exists
    private float healthPenalty;
    private float damageMultiplier;

    public OverclockStatusEffect(float penalty, float multiplier)
    {
        healthPenalty = penalty;
        damageMultiplier = multiplier;
    }

    public void OnApply(Entity subject)
    {
        int damage = Mathf.CeilToInt(subject.Health.MaxHealth * healthPenalty);
        subject.ReceiveDamage(null, damage);
    }

    public int OnCalculateDamageAdditive(int damage)
    {
        return damage;
    }

    public float OnCalculateDamageMultiplicative(float damage)
    {
        return damage * damageMultiplier;
    }

    public void OnTurnEnd(Entity subject)
    {
        subject.RemoveStatusEffect(this);
        Debug.Log("Overclock wore off");
    }
}
