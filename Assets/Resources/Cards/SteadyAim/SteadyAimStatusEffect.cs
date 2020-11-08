using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteadyAimStatusEffect : StatusEffect, IStatusTurnEndEventHandler, IStatusCalculateDamageEventHandler
{
    public override string GetName()
    {
        return "SteadyAim";
    }
    public override string GetDescription()
    {

        return string.Format("For one turn all your attacks will deal crit {0}x damage", 2f);
           
    }
    public int OnCalculateDamageAdditive(int damage)
    {
        return damage;
    }
    public float OnCalculateDamageMultiplicative(float damage)
    {
        if (GameplayContext.ActiveCard?.cardData.cardID == 14) //basic shot
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
