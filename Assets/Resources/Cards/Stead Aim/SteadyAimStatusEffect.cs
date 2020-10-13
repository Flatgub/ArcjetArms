using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteadyAimStatusEffect : StatusEffect, IStatusAttackEventHandler,
    IStatusCalculateDamageEventHandler, IStackableStatus
{
   
    public int OnCalculateDamageAdditive(int damage)
    {
        return damage;
    }
    public float OnCalculateDamageMultiplicative(float damage)
    {
        if (GameplayContext.ActiveCard?.cardData.cardID == 14)
        {
            return damage * 2f;
        }

        return damage;
    }
    public void OnTurnEnd(Entity subject)
    {
        subject.RemoveStatusEffect(this);
    }
}
