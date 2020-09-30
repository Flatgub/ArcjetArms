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
    }

    public override string GetName()
    {
        return "Charged";
    }

    public override string GetDescription()
    {
        string noun = "punch";
        if (remainingCharges != 1) 
        {
            noun = remainingCharges.Colored(Color.yellow) + " punches";
        }
        return string.Format("The next {0} will deal double damage.", noun);
    }

    public void OnAttack(Entity subject, Entity target)
    {
        if (GameplayContext.ActiveCard?.cardData.cardID == 1)
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
        if (GameplayContext.ActiveCard?.cardData.cardID == 1)
        {
            return damage * 2f;
        }

        return damage;
    }
}
