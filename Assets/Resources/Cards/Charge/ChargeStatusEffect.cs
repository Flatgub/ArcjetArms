using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeStatusEffect : StatusEffect, IStatusAttackEventHandler, 
    IStatusCalculateDamageEventHandler, IStackableStatus
{
    int remainingCharges = 1;

    //if charge is applied again, we gain an extra charge
    public void GainStack(IStackableStatus other) 
    {
        remainingCharges++;
        Debug.Log("charge now has " + remainingCharges + " charges");
    }

    public void OnAttack(Entity subject, Entity target)
    {
        if (GameplayContext.ActiveCard?.cardData.title == "Punch")
        {
            remainingCharges--;
            if (remainingCharges == 0)
            {
                Debug.Log("charge wore off");
                subject.RemoveStatusEffect(this);
            }
        }
    }

    public int OnCalculateDamageAdditive(int damage)
    {
        return damage;
    }

    public float OnCalculateDamageMultiplicative(float damage)
    {
        if (GameplayContext.ActiveCard?.cardData.title == "Punch")
        {
            return damage * 2f;
        }

        return damage;
    }
}
