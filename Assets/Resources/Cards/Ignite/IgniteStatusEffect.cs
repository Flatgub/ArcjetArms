﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgniteStatusEffect : StatusEffect, IStatusAttackEventHandler,
    IStatusCalculateDamageEventHandler, IStackableStatus
{
    private int percentageIncrease;

    public IgniteStatusEffect(int percentage)
    {
        percentageIncrease = percentage;
    }

    public void GainStack(IStackableStatus other)
    {
        IgniteStatusEffect o = other as IgniteStatusEffect;
        percentageIncrease += o.percentageIncrease;
    }

    public void OnAttack(Entity subject, Entity target)
    {
        subject.RemoveStatusEffect(this);
        Debug.Log("Ignite wore off");
    }

    public int OnCalculateDamageAdditive(int damage)
    {
        return damage;
    }

    public float OnCalculateDamageMultiplicative(float damage)
    {
        float multiplier = (100 + percentageIncrease) / 100f;
        return damage * multiplier;
    }
}
