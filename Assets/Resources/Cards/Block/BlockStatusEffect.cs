using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockStatusEffect : StatusEffect, IStatusReceiveDamageEventHandler
{
    private float blockAmount;

    public BlockStatusEffect(float amount)
    {
        blockAmount = amount;
    }

    public void OnAttacked(Entity subject, Entity attacker)
    {
        subject.RemoveStatusEffect(this);
        Debug.Log("block wore off");
    }

    public int OnReceivingDamage(int baseDamage)
    {
        return Mathf.CeilToInt(baseDamage * blockAmount);
    }
}
