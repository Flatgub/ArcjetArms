using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmourLockStatusEffect : StatusEffect, IStatusReceiveDamageEventHandler
{
    private float blockAmount;

    public ArmourLockStatusEffect(float amount)
    {
        blockAmount = amount;
    }

    public override string GetName()
    {
        return "Blocking";
    }

    public override string GetDescription()
    {
        string amount = Mathf.CeilToInt((1f - blockAmount) * 100).ToString();
        return string.Format("Playing this card ends your turn, you resist up to {0} incoming damage from the next attack.",
            (amount + "%").Colored(Color.green));
    }

    public void OnAttacked(Entity subject, Entity attacker)
    {
        subject.RemoveStatusEffect(this);
    }

    public int OnReceivingDamage(int baseDamage)
    {
        return Mathf.CeilToInt(baseDamage * blockAmount);
    }
}
